using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	public class SqlStorageFilterParameterBuilder
	{
		private readonly Dictionary<FieldValueType, HashSet<string>> _attributes = new Dictionary<FieldValueType, HashSet<string>>();

		private readonly HashSet<string> _systemFields = new HashSet<string>();

		private readonly StringBuilder _filterExpression = new StringBuilder();

		private readonly StringBuilder _havingExpression = new StringBuilder();

		public string FilterExpression => _filterExpression.ToString();

		public string HavingExpression
		{
			get
			{
				if (_havingExpression.Length <= 0)
				{
					return "";
				}
				return _havingExpression.ToString();
			}
		}

		public IEnumerable<string> GetFieldValueTypeAttributes(FieldValueType type)
		{
			if (_attributes.ContainsKey(type))
			{
				return _attributes[type];
			}
			return new List<string>();
		}

		public IEnumerable<string> GetSystemFields()
		{
			return _systemFields;
		}

		private static string TranslateFieldName(string fieldName)
		{
			switch (fieldName.ToLowerInvariant())
			{
			case "chd":
				return "tu.change_date";
			case "chu":
				return "tu.change_user";
			case "usd":
				return "tu.last_used_date";
			case "usu":
				return "tu.last_used_user";
			case "usc":
				return "tu.usage_counter";
			case "crd":
				return "tu.creation_date";
			case "cru":
				return "tu.creation_user";
			case "src":
				return "tu.source_segment";
			case "trg":
				return "tu.target_segment";
			case "sourceplainlength":
				return "tu.source_segment";
			case "targetplainlength":
				return "tu.target_segment";
			case "confirmationlevel":
				return "tu.confirmationLevel";
			default:
				return fieldName;
			}
		}

		private void ProcessComposedExpression(ComposedExpression expression)
		{
			if (expression.LeftOperand != null)
			{
				ProcessFilterExpression(expression.LeftOperand);
			}
			switch (expression.Op)
			{
			case ComposedExpression.Operator.Not:
				_filterExpression.Append(" NOT ");
				if (_havingExpression.Length > 0)
				{
					_havingExpression.Append(" NOT ");
				}
				break;
			case ComposedExpression.Operator.And:
				_filterExpression.Append(" AND ");
				if (_havingExpression.Length > 0)
				{
					_havingExpression.Append(" AND ");
				}
				break;
			case ComposedExpression.Operator.Or:
				_filterExpression.Append(" OR ");
				if (_havingExpression.Length > 0)
				{
					_havingExpression.Append(" OR ");
				}
				break;
			}
			if (expression.RightOperand != null)
			{
				ProcessFilterExpression(expression.RightOperand);
			}
		}

		private static int GetDbConfirmationLevel(string confirmationLevel)
		{
			switch (confirmationLevel.ToLowerInvariant())
			{
			case "unspecified":
				return 0;
			case "draft":
				return 1;
			case "translated":
				return 2;
			case "rejectedtranslation":
				return 3;
			case "approvedtranslation":
				return 4;
			case "rejectedsignoff":
				return 5;
			case "approvedsignoff":
				return 6;
			default:
				return 255;
			}
		}

		private void ProcessAtomicExpression(AtomicExpression expression)
		{
			string columnName = TranslateFieldName(expression.Value.Name);
			string text = columnName + "_total";
			if (columnName.StartsWith("tu."))
			{
				if (!_systemFields.Contains(columnName))
				{
					_systemFields.Add(columnName);
				}
				columnName = columnName.Insert(3, "[") + "]";
				if (expression.Value.Name == "sourceplainlength" || expression.Value.Name == "targetplainlength")
				{
					columnName = "DataLength(" + columnName + ")";
				}
				text = text.Insert(3, "[") + "]";
			}
			else
			{
				if (!_attributes.ContainsKey(expression.Value.ValueType))
				{
					_attributes.Add(expression.Value.ValueType, new HashSet<string>());
				}
				if (!_attributes[expression.Value.ValueType].Contains(columnName))
				{
					_attributes[expression.Value.ValueType].Add(columnName);
				}
				columnName = "[" + columnName + "]";
				text = "[" + text + "]";
			}
			SinglePicklistFieldValue singlePicklistFieldValue = expression.Value as SinglePicklistFieldValue;
			if (singlePicklistFieldValue != null && columnName == "tu.[confirmationLevel]")
			{
				int dbConfirmationLevel = GetDbConfirmationLevel(singlePicklistFieldValue.Value.Name);
				singlePicklistFieldValue.Value.Name = dbConfirmationLevel.ToString();
			}
			switch (expression.Op)
			{
			case AtomicExpression.Operator.Equal:
			{
				FieldValue value = expression.Value;
				SingleStringFieldValue singleStringFieldValue4 = value as SingleStringFieldValue;
				if (singleStringFieldValue4 == null)
				{
					MultipleStringFieldValue multipleStringFieldValue3 = value as MultipleStringFieldValue;
					if (multipleStringFieldValue3 == null)
					{
						SinglePicklistFieldValue singlePicklistFieldValue3 = value as SinglePicklistFieldValue;
						if (singlePicklistFieldValue3 == null)
						{
							MultiplePicklistFieldValue multiplePicklistFieldValue3 = value as MultiplePicklistFieldValue;
							if (multiplePicklistFieldValue3 == null)
							{
								DateTimeFieldValue dateTimeFieldValue2 = value as DateTimeFieldValue;
								if (dateTimeFieldValue2 == null)
								{
									IntFieldValue intFieldValue2 = value as IntFieldValue;
									if (intFieldValue2 != null)
									{
										_filterExpression.Append($"({columnName} = {intFieldValue2.Value})");
									}
								}
								else
								{
									_filterExpression.Append($"({columnName} = '{dateTimeFieldValue2.Value:yyyy-MM-dd HH:mm:ss}')");
								}
							}
							else
							{
								string text2 = string.Join(",", multiplePicklistFieldValue3.Values.Select((PicklistItem x) => "'" + x.Name.Replace("'", "''") + "'"));
								_filterExpression.Append($"({columnName} IN ({text2})) AND {text} = {multiplePicklistFieldValue3.Values.Count}");
								_havingExpression.Append($" count(distinct {columnName}) = {multiplePicklistFieldValue3.Values.Count}");
							}
						}
						else
						{
							_filterExpression.AppendFormat("({0} = '{1}')", columnName, singlePicklistFieldValue3.Value.Name.Replace("'", "''"));
						}
					}
					else
					{
						string text3 = string.Join(",", multipleStringFieldValue3.Values.Select((string x) => "'" + x.Replace("'", "''") + "'"));
						_filterExpression.Append($"({columnName} IN ({text3})) AND {text} = {multipleStringFieldValue3.Values.Count()}");
						_havingExpression.Append($" count(distinct {columnName}) = {multipleStringFieldValue3.Values.Count()}");
					}
				}
				else if (columnName == "tu.[source_segment]" || columnName == "tu.[target_segment]")
				{
					_filterExpression.AppendFormat("(CONVERT(NVARCHAR(MAX), {0}) = '{1}')", columnName, singleStringFieldValue4.Value.Replace("'", "''"));
				}
				else
				{
					_filterExpression.AppendFormat("({0} = '{1}')", columnName, singleStringFieldValue4.Value.Replace("'", "''"));
				}
				break;
			}
			case AtomicExpression.Operator.Smaller:
			case AtomicExpression.Operator.SmallerEqual:
			case AtomicExpression.Operator.Greater:
			case AtomicExpression.Operator.GreaterEqual:
			{
				string arg4 = "";
				switch (expression.Op)
				{
				case AtomicExpression.Operator.Greater:
					arg4 = ">";
					break;
				case AtomicExpression.Operator.Smaller:
					arg4 = "<";
					break;
				case AtomicExpression.Operator.GreaterEqual:
					arg4 = ">=";
					break;
				case AtomicExpression.Operator.SmallerEqual:
					arg4 = "<=";
					break;
				}
				FieldValue value = expression.Value;
				SingleStringFieldValue singleStringFieldValue5 = value as SingleStringFieldValue;
				if (singleStringFieldValue5 == null)
				{
					DateTimeFieldValue dateTimeFieldValue3 = value as DateTimeFieldValue;
					if (dateTimeFieldValue3 == null)
					{
						IntFieldValue intFieldValue3 = value as IntFieldValue;
						if (intFieldValue3 != null)
						{
							_filterExpression.AppendFormat("({0} {1} {2})", columnName, arg4, intFieldValue3.Value);
						}
					}
					else
					{
						_filterExpression.AppendFormat("({0} {1} '{2:yyyy-MM-dd HH:mm:ss}')", columnName, arg4, dateTimeFieldValue3.Value);
					}
				}
				else
				{
					_filterExpression.AppendFormat("({0} {1} '{2}')", columnName, arg4, singleStringFieldValue5.Value.Replace("'", "''"));
				}
				break;
			}
			case AtomicExpression.Operator.NotEqual:
			{
				FieldValue value = expression.Value;
				SingleStringFieldValue singleStringFieldValue3 = value as SingleStringFieldValue;
				if (singleStringFieldValue3 == null)
				{
					DateTimeFieldValue dateTimeFieldValue = value as DateTimeFieldValue;
					if (dateTimeFieldValue == null)
					{
						IntFieldValue intFieldValue = value as IntFieldValue;
						if (intFieldValue == null)
						{
							SinglePicklistFieldValue singlePicklistFieldValue2 = value as SinglePicklistFieldValue;
							if (singlePicklistFieldValue2 != null)
							{
								_filterExpression.AppendFormat("({0} != '{1}')", columnName, singlePicklistFieldValue2.Value.Name.Replace("'", "''"));
							}
						}
						else
						{
							_filterExpression.AppendFormat("({0} != {1})", columnName, intFieldValue.Value);
						}
					}
					else
					{
						_filterExpression.AppendFormat("({0} != '{1:yyyy-MM-dd HH:mm:ss}')", columnName, dateTimeFieldValue.Value);
					}
				}
				else
				{
					_filterExpression.AppendFormat("({0} != '{1}')", columnName, singleStringFieldValue3.Value.Replace("'", "''"));
				}
				break;
			}
			case AtomicExpression.Operator.Contains:
			{
				FieldValue value = expression.Value;
				SingleStringFieldValue singleStringFieldValue2 = value as SingleStringFieldValue;
				if (singleStringFieldValue2 == null)
				{
					MultipleStringFieldValue multipleStringFieldValue2 = value as MultipleStringFieldValue;
					if (multipleStringFieldValue2 == null)
					{
						MultiplePicklistFieldValue multiplePicklistFieldValue2 = value as MultiplePicklistFieldValue;
						if (multiplePicklistFieldValue2 != null)
						{
							string arg3 = string.Join(",", multiplePicklistFieldValue2.Values.Select((PicklistItem x) => "'" + x.Name.Replace("'", "''") + "'"));
							_filterExpression.AppendFormat("({0} IN ({1}))", columnName, arg3);
							_havingExpression.AppendFormat(" count(distinct {0}) >= {1}", columnName, multiplePicklistFieldValue2.Values.Count);
						}
					}
					else
					{
						string format = string.Join(" OR ", multipleStringFieldValue2.Values.Select((string x) => "(" + columnName + " like '%" + x.Replace("'", "''") + "%')"));
						_filterExpression.AppendFormat(format);
						_havingExpression.AppendFormat(" count(distinct {0}) >= {1}", columnName, multipleStringFieldValue2.Values.Count());
					}
				}
				else
				{
					_filterExpression.AppendFormat("({0} like N'%{1}%')", columnName, singleStringFieldValue2.Value.Replace("'", "''"));
				}
				break;
			}
			case AtomicExpression.Operator.ContainsNot:
			{
				FieldValue value = expression.Value;
				SingleStringFieldValue singleStringFieldValue = value as SingleStringFieldValue;
				if (singleStringFieldValue == null)
				{
					MultipleStringFieldValue multipleStringFieldValue = value as MultipleStringFieldValue;
					if (multipleStringFieldValue == null)
					{
						MultiplePicklistFieldValue multiplePicklistFieldValue = value as MultiplePicklistFieldValue;
						if (multiplePicklistFieldValue != null)
						{
							string arg = string.Join(",", multiplePicklistFieldValue.Values.Select((PicklistItem x) => "'" + x.Name.Replace("'", "''") + "'"));
							_filterExpression.AppendFormat("({0} IN ({1}))", columnName, arg);
							_havingExpression.AppendFormat(" count(distinct {0}) = 0", columnName);
						}
					}
					else
					{
						string arg2 = string.Join(",", multipleStringFieldValue.Values.Select((string x) => "'" + x.Replace("'", "''") + "'"));
						_filterExpression.AppendFormat("({0} IN ({1}))", columnName, arg2);
						_havingExpression.AppendFormat(" count(distinct {0}) = 0", columnName);
					}
				}
				else
				{
					_filterExpression.AppendFormat("({0} not like N'%{1}%')", columnName, singleStringFieldValue.Value.Replace("'", "''"));
				}
				break;
			}
			}
		}

		public void ProcessFilterExpression(FilterExpression filter)
		{
			ComposedExpression composedExpression = filter as ComposedExpression;
			if (composedExpression == null)
			{
				AtomicExpression atomicExpression = filter as AtomicExpression;
				if (atomicExpression != null)
				{
					ProcessAtomicExpression(atomicExpression);
				}
			}
			else
			{
				ProcessComposedExpression(composedExpression);
			}
		}
	}
}
