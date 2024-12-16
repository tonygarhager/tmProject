
----- Translation units
CREATE TABLE dbo.translation_units_%%(
	id INT IDENTITY NOT NULL PRIMARY KEY,
	guid UNIQUEIDENTIFIER NOT NULL,
	source_hash BIGINT NOT NULL,
	source_segment NTEXT,
	target_hash BIGINT NOT NULL,
	target_segment NTEXT,
	creation_date DATETIME NOT NULL,
	creation_user NVARCHAR(255) NOT NULL,
	change_date DATETIME NOT NULL,
	change_user NVARCHAR(255) NOT NULL,
	last_used_date DATETIME NOT NULL,
	last_used_user NVARCHAR(255) NOT NULL,
	usage_counter INT NOT NULL,
	source_token_data VARBINARY(MAX),
	target_token_data VARBINARY(MAX),
	tokenization_sig_hash BIGINT,
	serialization_version INT NOT NULL,
	source_tags VARBINARY(MAX),
	target_tags VARBINARY(MAX),
	format tinyint not null,	
	origin tinyint not null,
	confirmationLevel tinyint not null
)

GO

CREATE TABLE dbo.translation_unit_alignment_data_%%(
	translation_unit_id INT NOT NULL CONSTRAINT FK_tua_tu1_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE,
	alignment_data VARBINARY(MAX),
	align_model_date DATETIME,
	insert_date DATETIME,
	unalignedLastScheduled DATETIME

)
GO

CREATE CLUSTERED INDEX [tua_tu_clustered_%%] ON [dbo].[translation_unit_alignment_data_%%]
(
	[translation_unit_id] ASC
)
GO

CREATE INDEX tus_hash_%% ON dbo.translation_units_%%(source_hash, target_hash)
GO

CREATE INDEX [NonCluteredIndex__fragment_hash] on dbo.translation_units_%%([source_hash] ASC, [change_date] DESC, [id] DESC) with(fillfactor=80)
GO

CREATE INDEX [NonCluteredIndex__source_target_hash] on dbo.translation_units_%%([source_hash] ASC, [id] ASC, [target_hash] ASC)	with(fillfactor=80)
GO

CREATE INDEX tus_insert_date_%% ON dbo.translation_unit_alignment_data_%%(insert_date)
GO

CREATE INDEX tus_align_model_date_%% ON dbo.translation_unit_alignment_data_%%(align_model_date)
GO

CREATE INDEX [NonCluteredIndex__translation_unit_id] on dbo.translation_unit_alignment_data_%% ([translation_unit_id])
INCLUDE ([alignment_data],[align_model_date],[insert_date], [unalignedLastScheduled])
with(fillfactor=80)
GO

CREATE NONCLUSTERED INDEX [tua_lsched_nonclustered_%%] 
ON [dbo].[translation_unit_alignment_data_%%] ( [unalignedLastScheduled] ASC ) 
INCLUDE ( [alignment_data] ) 
WHERE ( [unalignedLastScheduled] IS NULL AND [alignment_data] IS NULL ) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) 
GO 

CREATE NONCLUSTERED INDEX [tua_lsched2_nonclustered_%%] 
ON [dbo].[translation_unit_alignment_data_%%] ( [unalignedLastScheduled] ASC ) 
INCLUDE ( [alignment_data] ) 
WHERE ( [alignment_data] IS NULL ) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) 
GO 

CREATE NONCLUSTERED INDEX [tua_lsched_idate_nonclustered_%%] 
ON [dbo].[translation_unit_alignment_data_%%] ( [unalignedLastScheduled] ASC, [insert_date] ASC ) 
INCLUDE ( [align_model_date] ) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) 
GO 

CREATE INDEX [NonCluteredIndex__dates] on dbo.translation_unit_alignment_data_%%([insert_date],[align_model_date],[unalignedLastScheduled])	with(fillfactor=80)
GO

CREATE INDEX [NonCluteredIndex__alignment_data_null] on dbo.translation_unit_alignment_data_%%([translation_unit_id] ASC,[unalignedLastScheduled]) with(fillfactor=80)
GO

CREATE INDEX tus_tokenization_sig_hash_%% ON dbo.translation_units_%%(tokenization_sig_hash)
GO

CREATE TABLE dbo.translation_unit_contexts_%%(
	translation_unit_id INT NOT NULL CONSTRAINT FK_tuc_tu1_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE,
	context1 BIGINT NOT NULL,
	context2 BIGINT NOT NULL
)

GO


-- creating a PK over all three columns will make export very slow (clustered index scan) for heavily attributed TMs 

CREATE INDEX IDX_translation_unit_contexts_tu_id_%% ON dbo.translation_unit_contexts_%%(translation_unit_id, context1, context2)
GO

CREATE INDEX [NonCluteredIndex__context1] on dbo.translation_unit_contexts_%%([context1]) INCLUDE([translation_unit_id], [context2]) with(fillfactor=50)
GO

CREATE INDEX [NonCluteredIndex__context2] on dbo.translation_unit_contexts_%%([context2]) INCLUDE([translation_unit_id], [context1]) with(fillfactor=50)
GO

CREATE TABLE dbo.translation_unit_idcontexts_%%(
	translation_unit_id INT NOT NULL CONSTRAINT FK_tuidc_tu_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE,
	idcontext NVARCHAR(250),
)

GO

CREATE UNIQUE INDEX IDX_translation_unit_idcontexts_%% ON dbo.translation_unit_idcontexts_%%(translation_unit_id, idcontext)
GO

----- Source text translatable fragments for DTA subsegment recall
CREATE TABLE dbo.translation_unit_fragments_%%(
	translation_unit_id INT NOT NULL 
		CONSTRAINT FK_tuf_tu1_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE,
	fragment_hash bigint NOT NULL
	
)

GO

CREATE INDEX idx_tufragments_ids_%% ON dbo.translation_unit_fragments_%%(translation_unit_id)
GO
CREATE INDEX idx_tufragments_hashes_%% ON dbo.translation_unit_fragments_%%(fragment_hash)
GO
CREATE INDEX [NonCluteredIndex__fragment_hash] on dbo.translation_unit_fragments_%%([fragment_hash], [translation_unit_id])	with(fillfactor=80)
GO



----- Attribute declarations
CREATE TABLE dbo.attributes_%%(
	id INT IDENTITY NOT NULL PRIMARY KEY,
	guid UNIQUEIDENTIFIER NOT NULL,
	name NVARCHAR(400) NOT NULL,
	type INT NOT NULL
CONSTRAINT CK_a_%% UNIQUE (
	name
)
)

GO	

----- String attribute values
CREATE TABLE dbo.string_attributes_%%(
	attribute_id INT NOT NULL 
		CONSTRAINT FK_sa_a_%% REFERENCES dbo.attributes_%%(id) ON DELETE CASCADE,
	value NVARCHAR(MAX) NOT NULL,
	translation_unit_id INT NOT NULL 
		CONSTRAINT FK_sa_tu_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE
)

GO

-- creating a PK over all three columns will make export very slow (clustered index scan) for heavily attributed TMs 

CREATE INDEX [NonCluteredIndex__translation_unit_id] on dbo.string_attributes_%% ([translation_unit_id], [attribute_id]) INCLUDE ([value]) with(fillfactor=80)

GO	

----- Numeric attribute values
CREATE TABLE dbo.numeric_attributes_%%(
	attribute_id INT NOT NULL CONSTRAINT FK_na_a_%% REFERENCES dbo.attributes_%%(id) ON DELETE CASCADE,
	value INT NOT NULL,
	translation_unit_id INT NOT NULL CONSTRAINT FK_na_tu_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE
)

GO

-- creating a PK over all three columns will make export very slow (clustered index scan) for heavily attributed TMs 

CREATE INDEX [NonCluteredIndex__translation_unit_id] on dbo.numeric_attributes_%% ([translation_unit_id], [attribute_id]) INCLUDE ([value]) with(fillfactor=80)

GO	

----- Date attribute values
CREATE TABLE dbo.date_attributes_%%(
	attribute_id INT NOT NULL CONSTRAINT FK_da_a_%% REFERENCES dbo.attributes_%%(id) ON DELETE CASCADE,
	value DATETIME NOT NULL,
	translation_unit_id INT NOT NULL CONSTRAINT FK_da_tu_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE
)

GO

-- creating a PK over all three columns will make export very slow (clustered index scan) for heavily attributed TMs 

CREATE INDEX [NonCluteredIndex__translation_unit_id] on dbo.date_attributes_%% ([translation_unit_id], [attribute_id]) INCLUDE ([value]) with(fillfactor=80)

GO	

----- Picklist attribute values
CREATE TABLE dbo.picklist_values_%%(
	id INT IDENTITY NOT NULL PRIMARY KEY,
	guid UNIQUEIDENTIFIER NOT NULL,
	attribute_id INT NOT NULL CONSTRAINT FK_pv_a_%% REFERENCES dbo.attributes_%%(id) ON DELETE CASCADE,
	value NVARCHAR(MAX) NOT NULL
)

GO

CREATE INDEX [NonCluteredIndex__attribute_id] on dbo.picklist_values_%%([attribute_id])	include([id],[guid],[value]) with(fillfactor=80)
GO


----- Picklist attributes
CREATE TABLE dbo.picklist_attributes_%%(
	translation_unit_id INT NOT NULL CONSTRAINT FK_pa_tu_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE,
	picklist_value_id INT NOT NULL CONSTRAINT FK_pa_pv_%% REFERENCES dbo.picklist_values_%%(id) ON DELETE CASCADE,
CONSTRAINT PK_pa_%% PRIMARY KEY CLUSTERED (
	translation_unit_id,
	picklist_value_id
)
)

GO

CREATE INDEX idx_picklist_attributes_%% ON dbo.picklist_attributes_%% (translation_unit_id);

GO

----- Fuzzy indexes
CREATE TABLE dbo.fuzzy_index1_%%(
	feature INT NOT NULL,
	length INT NOT NULL,
	translation_unit_id INT NOT NULL, -- CONSTRAINT FK_fi1_tu REFERENCES dbo.translation_units(id) ON DELETE CASCADE,
CONSTRAINT PK_fi1_%% PRIMARY KEY CLUSTERED (
	feature,
	length,
	translation_unit_id
)
)

GO	

CREATE INDEX fi1_address_%% ON dbo.fuzzy_index1_%%(translation_unit_id, feature)

GO

CREATE INDEX [NonCluteredIndex__length] on dbo.fuzzy_index1_%%([length],[translation_unit_id]) INCLUDE ([feature]) with(fillfactor=80)

GO

CREATE TABLE dbo.ff1_%%(
	feature INT NOT NULL,
	frequency INT NOT NULL,
CONSTRAINT PK_ff1_%% PRIMARY KEY CLUSTERED (
	feature,
	frequency
)
)

GO

CREATE TABLE dbo.fuzzy_index2_%%(
	feature INT NOT NULL,
	length INT NOT NULL,
	translation_unit_id INT NOT NULL, -- CONSTRAINT FK_fi2_tu REFERENCES dbo.translation_units(id) ON DELETE CASCADE,
CONSTRAINT PK_fi2_%% PRIMARY KEY CLUSTERED (
	feature,
	length,
	translation_unit_id
)
)

GO	

CREATE INDEX fi2_address_%% ON dbo.fuzzy_index2_%%(translation_unit_id, feature)

GO

CREATE INDEX [NonCluteredIndex__length] on dbo.fuzzy_index2_%%([length],[translation_unit_id]) INCLUDE ([feature]) with(fillfactor=80)

GO

CREATE TABLE dbo.ff2_%%(
	feature INT NOT NULL,
	frequency INT NOT NULL,
CONSTRAINT PK_ff2_%% PRIMARY KEY CLUSTERED (
	feature,
	frequency
)
)

GO

CREATE TABLE dbo.fuzzy_index4_%%(
	feature INT NOT NULL,
	length INT NOT NULL,
	translation_unit_id INT NOT NULL, -- CONSTRAINT FK_fi4_tu REFERENCES dbo.translation_units(id) ON DELETE CASCADE,
CONSTRAINT PK_fi4_%% PRIMARY KEY CLUSTERED (
	feature,
	length,
	translation_unit_id
)
)

GO	

CREATE INDEX fi4_address_%% ON dbo.fuzzy_index4_%%(translation_unit_id, feature)

GO

CREATE INDEX [NonCluteredIndex__length] on dbo.fuzzy_index4_%%([length],[translation_unit_id]) INCLUDE ([feature]) with(fillfactor=80)

GO

CREATE TABLE dbo.ff4_%%(
	feature INT NOT NULL,
	frequency INT NOT NULL,
CONSTRAINT PK_ff4_%% PRIMARY KEY CLUSTERED (
	feature,
	frequency
)
)

GO

CREATE TABLE dbo.fuzzy_index8_%%(
	feature INT NOT NULL,
	length INT NOT NULL,
	translation_unit_id INT NOT NULL, -- CONSTRAINT FK_fi8_tu REFERENCES dbo.translation_units(id) ON DELETE CASCADE,
CONSTRAINT PK_fi8_%% PRIMARY KEY CLUSTERED (
	feature,
	length,
	translation_unit_id
)
)

GO	

CREATE INDEX fi8_address_%% ON dbo.fuzzy_index8_%%(translation_unit_id, feature)

GO

CREATE INDEX [NonCluteredIndex__length] on dbo.fuzzy_index8_%%([length],[translation_unit_id]) INCLUDE ([feature]) with(fillfactor=80)

GO

CREATE TABLE dbo.ff8_%%(
	feature INT NOT NULL,
	frequency INT NOT NULL,
CONSTRAINT PK_ff8_%% PRIMARY KEY CLUSTERED (
	feature,
	frequency
)
)
GO

CREATE TABLE dbo.translation_unit_last_search_%%(
	translation_unit_id INT NOT NULL CONSTRAINT FK_lastSearch_tu_%% REFERENCES dbo.translation_units_%%(id) ON DELETE CASCADE,
	last_search_on DATETIME NOT NULL
)

GO

----------------

----- Attribute related

CREATE PROCEDURE dbo.add_attribute_%% @name NVARCHAR(MAX), @type INT, @guid UNIQUEIDENTIFIER AS
	SET NOCOUNT ON
	INSERT INTO dbo.attributes_%%(name, type, guid) VALUES(@name, @type, @guid)
	SELECT CAST(SCOPE_IDENTITY() AS INT)
GO

CREATE PROCEDURE dbo.get_attributes_%% @id INT = NULL, @name NVARCHAR(MAX) = NULL AS
	SET NOCOUNT ON
	IF @id IS NOT NULL
		SELECT id, guid, name, type, %% FROM dbo.attributes_%% WHERE id = @id
	ELSE IF @name IS NOT NULL
		SELECT id, guid, name, type, %% FROM dbo.attributes_%% WHERE name = @name
	ELSE
		SELECT id, guid, name, type, %% FROM dbo.attributes_%%
GO

CREATE PROCEDURE dbo.rename_attribute_%% @id INT, @newname NVARCHAR(MAX) AS
	UPDATE dbo.attributes_%% SET name = @newname WHERE id = @id
GO

CREATE PROCEDURE dbo.delete_attribute_%% @id INT = NULL, @name NVARCHAR(MAX) = NULL AS
	IF @id IS NOT NULL
		DELETE FROM dbo.attributes_%% WHERE id = @id
	ELSE IF @name IS NOT NULL
		DELETE FROM dbo.attributes_%% WHERE name = @name
GO

----- Picklist related
  
CREATE PROCEDURE dbo.add_picklist_value_%% @attribute_id INT, @value NVARCHAR(MAX), @guid UNIQUEIDENTIFIER AS
	SET NOCOUNT ON
	INSERT INTO dbo.picklist_values_%%(attribute_id, value, guid) VALUES(@attribute_id, @value, @guid)
	SELECT CAST(SCOPE_IDENTITY() AS INT)
GO

CREATE PROCEDURE dbo.get_attribute_picklist_%% @id INT = NULL, @attribute_id INT = NULL AS
	SET NOCOUNT ON
	IF @id IS NOT NULL
		SELECT id, guid, value FROM dbo.picklist_values_%% WHERE id = @id
	ELSE
		SELECT id, guid, value FROM dbo.picklist_values_%% WHERE attribute_id = @attribute_id
GO

CREATE PROCEDURE dbo.rename_picklist_value_%% @attribute_id INT, @oldname NVARCHAR(MAX), @newname NVARCHAR(MAX) AS
	UPDATE dbo.picklist_values_%% SET value = @newname WHERE attribute_id = @attribute_id AND value = @oldname
GO

CREATE PROCEDURE dbo.delete_picklist_value_%% @attribute_id INT, @value NVARCHAR(MAX) AS
	DELETE FROM dbo.picklist_values_%% WHERE attribute_id = @attribute_id AND value = @value
GO

----- TU related

CREATE PROCEDURE dbo.add_tu_%% @guid UNIQUEIDENTIFIER, 
	@source_hash BIGINT, @source_text NTEXT, 
	@target_hash BIGINT, @target_text NTEXT, 
	@crd DATETIME, @cru NVARCHAR(255), 
	@chd DATETIME, @chu NVARCHAR(255), @lud DATETIME, @luu NVARCHAR(255), 
	@usc INT = 0, @id INT = 0, 
	@data1 VARBINARY(MAX) = NULL, 
	@data2 VARBINARY(MAX) = NULL, 
	@data4 VARBINARY(MAX) = NULL,
	@data8 VARBINARY(MAX) = NULL,
	@source_token_data VARBINARY(MAX) = NULL,
	@target_token_data VARBINARY(MAX) = NULL,
	@alignment_data VARBINARY(MAX) = NULL,
	@align_model_date DATETIME = NULL,
	@insert_date DATETIME,
	@sig_hash BIGINT,
	@relaxed_hash BIGINT,
	@serialization_version INT,
	@source_tags VARBINARY(MAX) = NULL,
	@target_tags VARBINARY(MAX) = NULL,
	@format INT,
	@origin INT,	
	@confirmationLevel INT
AS
	SET NOCOUNT ON
	IF @id = 0 BEGIN  
	
		INSERT INTO dbo.translation_units_%%(guid, source_hash, source_segment, 
			target_hash, target_segment, 
			creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, tokenization_sig_hash,
			serialization_version, source_tags, target_tags, format, origin, confirmationLevel)
			VALUES(@guid, @source_hash, @source_text, @target_hash, @target_text, 
			@crd, @cru, @chd, @chu, @lud, @luu, @usc, @source_token_data, @target_token_data, @sig_hash,
			@serialization_version, @source_tags, @target_tags, @format, @origin, @confirmationLevel)
		
		SET @id = CAST(SCOPE_IDENTITY() AS INT)

	END 
	ELSE BEGIN  
	
		SET IDENTITY_INSERT dbo.translation_units_%% ON
		INSERT INTO dbo.translation_units_%%(id, guid, source_hash, source_segment, 
			target_hash, target_segment, 
			creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, tokenization_sig_hash,
			serialization_version, source_tags, target_tags, format, origin, confirmationLevel)
			VALUES(@id, @guid, @source_hash, @source_text, @target_hash, @target_text, 
			@crd, @cru, @chd, @chu, @lud, @luu, @usc, @source_token_data, @target_token_data, @sig_hash,
			@serialization_version, @source_tags, @target_tags, @format, @origin, @confirmationLevel)
		SET IDENTITY_INSERT dbo.translation_units_%% OFF
		
	END

	INSERT INTO dbo.translation_unit_alignment_data_%%(translation_unit_id, alignment_data, align_model_date, insert_date)
		VALUES(@id, @alignment_data, @align_model_date, @insert_date)

		
	IF @data1 IS NOT NULL
		exec dbo.add_features_%% @id, @data1, 1
	IF @data2 IS NOT NULL
		exec dbo.add_features_%% @id, @data2, 2
	IF @data4 IS NOT NULL
		exec dbo.add_features_%% @id, @data4, 4
	IF @data8 IS NOT NULL
		exec dbo.add_features_%% @id, @data8, 8

	INSERT INTO dbo.translation_unit_fragments_%%(translation_unit_id, fragment_hash) VALUES (@id,@relaxed_hash)

	SELECT @id
	
GO

CREATE PROCEDURE [dbo].[add_tu_batch_%%] 
	@tuBatch TuData4 readonly,
	@tuFeatures TuFeatures readonly,
	@tuStringAttributes TuStringAttributes readonly,
	@tuDateAttributes TuDateAttributes readonly,
	@tuNumericAttributes TuNumericAttributes readonly,
	@tuPickListAttributes TuNumericAttributes readonly,
	@tuContexts TuContexts readonly,
	@tuIdContexts TuIdContexts readonly
AS
	SET NOCOUNT ON

	-- find ids of tus where child data needs clearing before re-adding
	-- i.e. tus where we are performing partial update, in which case tu row
	-- has not been deleted
	DECLARE @IdsKept TABLE(id INT not null)
	INSERT INTO @IdsKept(id) SELECT id from @tuBatch WHERE importType = 2

	-- clear child tables that will be rewritten for those

	DELETE FROM string_attributes_%% 
	WHERE translation_unit_id IN (SELECT id FROM @IdsKept)
	DELETE FROM date_attributes_%% 
	WHERE translation_unit_id IN (SELECT id FROM @IdsKept)
	DELETE FROM numeric_attributes_%% 
	WHERE translation_unit_id IN (SELECT id FROM @IdsKept)
	DELETE FROM picklist_attributes_%% 
	WHERE translation_unit_id IN (SELECT id FROM @IdsKept)


	--new tus
	INSERT INTO dbo.translation_units_%%(guid, source_hash, source_segment, 
		target_hash, target_segment, 
		creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, tokenization_sig_hash,
		serialization_version, source_tags, target_tags , format, origin, confirmationLevel)
	SELECT 	guid, source_hash, source_segment, 
		target_hash, target_segment, 
		creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, tokenization_sig_hash,
		serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM @tuBatch
	WHERE importType = 0	
	
	-- updated tus 
	SET IDENTITY_INSERT dbo.translation_units_%% ON
	INSERT INTO dbo.translation_units_%%(id, guid, source_hash, source_segment, 
		target_hash, target_segment, 
		creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, tokenization_sig_hash,
		serialization_version, source_tags, target_tags , format, origin, confirmationLevel)
	SELECT 	id, guid, source_hash, source_segment, 
		target_hash, target_segment, 
		creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, tokenization_sig_hash,
		serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM @tuBatch
	WHERE importType = 1	
	SET IDENTITY_INSERT dbo.translation_units_%% OFF

	--updates tus explicit
	UPDATE
		TableToUpdate
	SET		
		TableToUpdate.change_date = TableImport.change_date,
		TableToUpdate.change_user = TableImport.change_user,
		TableToUpdate.last_used_date = TableImport.last_used_date,
		TableToUpdate.last_used_user = TableImport.last_used_user,
		TableToUpdate.usage_counter = TableImport.usage_counter,
		TableToUpdate.format = TableImport.format,
		TableToUpdate.origin = TableImport.origin,
		TableToUpdate.confirmationLevel = TableImport.confirmationLevel
	FROM
		dbo.translation_units_%%  AS TableToUpdate
		INNER JOIN @tuBatch AS TableImport
			ON TableToUpdate.id = TableImport.id	
	WHERE TableImport.importType = 2


	--update FGA data-----------------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO dbo.translation_unit_alignment_data_%%(translation_unit_id, alignment_data, align_model_date, insert_date)
	SELECT tus.id, b.alignment_data, b.align_model_date, b.insert_date
		FROM @tuBatch b INNER JOIN translation_units_%% (nolock) tus ON b.guid = tus.guid
		WHERE b.importType <=1

	INSERT INTO dbo.translation_unit_fragments_%%(translation_unit_id, fragment_hash) 
	SELECT tus.id, b.relaxed_hash 
		FROM @tuBatch b INNER JOIN translation_units_%% (nolock) tus ON b.guid = tus.guid
		WHERE b.importType <=1
	
	--update index-----------------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO dbo.fuzzy_index1_%%(feature, length, translation_unit_id)  
		SELECT feature, length, tus.id
			FROM @tuFeatures f INNER JOIN translation_units_%% (nolock) tus ON f.guid = tus.guid
			WHERE f.type = 1

	INSERT INTO dbo.fuzzy_index2_%%(feature, length, translation_unit_id)  
		SELECT feature, length, tus.id
			FROM @tuFeatures f INNER JOIN translation_units_%% (nolock) tus ON f.guid = tus.guid
			WHERE f.type = 2

	INSERT INTO dbo.fuzzy_index4_%%(feature, length, translation_unit_id)  
		SELECT feature, length, tus.id
			FROM @tuFeatures f INNER JOIN translation_units_%% (nolock) tus ON f.guid = tus.guid
			WHERE f.type = 4

	INSERT INTO dbo.fuzzy_index8_%%(feature, length, translation_unit_id)  
		SELECT feature, length, tus.id
			FROM @tuFeatures f INNER JOIN translation_units_%% (nolock) tus ON f.guid = tus.guid
			WHERE f.type = 8

	--update attributes-----------------------------------------------------------------------------------------------------------------------------------------------
	
	INSERT INTO dbo.string_attributes_%%(attribute_id, value, translation_unit_id) 
	SELECT a.attribute_id, a.value, tus.id
		FROM @TuStringAttributes a INNER JOIN translation_units_%% (nolock) tus ON a.guid = tus.guid
	
	INSERT INTO dbo.date_attributes_%%(attribute_id, value, translation_unit_id) 
	SELECT a.attribute_id, a.value, tus.id
		FROM @TuDateAttributes a INNER JOIN translation_units_%% (nolock) tus ON a.guid = tus.guid

	INSERT INTO dbo.picklist_attributes_%%(picklist_value_id, translation_unit_id)
	SELECT a.value, tus.id
		FROM @tuPickListAttributes a INNER JOIN translation_units_%% (nolock) tus ON a.guid = tus.guid


	INSERT INTO dbo.numeric_attributes_%%(attribute_id, value, translation_unit_id) 
	SELECT a.attribute_id, a.value, tus.id
		FROM @tuNumericAttributes  a INNER JOIN translation_units_%% (nolock) tus ON a.guid = tus.guid

	--TuContexts -----------------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO dbo.translation_unit_contexts_%%(context1, context2, translation_unit_id) 
	SELECT c.context1, c.context2, tus.id
		FROM @tuContexts c INNER JOIN translation_units_%% (nolock) tus ON c.guid = tus.guid

	--TuIDContexts-----------------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO dbo.translation_unit_idcontexts_%%(idcontext, translation_unit_id) 
	SELECT c.idcontext, tus.id
		FROM @tuIdContexts c INNER JOIN translation_units_%% (nolock) tus ON c.guid = tus.guid


	-- return the int ids that have changed
	SELECT b.guid, tus.id
	FROM @tuBatch b INNER JOIN translation_units_%% (nolock) tus ON b.guid = tus.guid
		WHERE b.importType = 0

GO

CREATE PROCEDURE dbo.get_tu_%% @id INT,
	@returnIdContext INT AS
	SET NOCOUNT ON
	SELECT id, guid, %%, source_hash, source_segment, 0, 0, 
		target_hash, target_segment, 0, 0, creation_date, 
		creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
		FROM dbo.translation_units_%% WHERE id = @id

		   SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value
			  FROM date_attributes_%% da INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id
			  WHERE da.translation_unit_id = @id
			  ORDER BY da.attribute_id;

		   SELECT sa.translation_unit_id, sa.attribute_id,a.name, a.type, sa.value
			  FROM string_attributes_%% sa INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
			  WHERE sa.translation_unit_id = @id
			  ORDER BY sa.attribute_id;

		  SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value
			  FROM numeric_attributes_%% na INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
			  WHERE na.translation_unit_id = @id
			  ORDER BY na.attribute_id;

		  SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id
			  FROM picklist_attributes_%% pa 
					INNER JOIN picklist_values_%% pv ON pv.id = pa.picklist_value_id
					INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
			  WHERE pa.translation_unit_id = @id
			  ORDER BY pv.attribute_id;

		  SELECT translation_unit_id, context1, context2
			  FROM translation_unit_contexts_%%
			  WHERE translation_unit_id = @id

	IF @returnIdContext = 1		
		SELECT translation_unit_id, idcontext
			  FROM translation_unit_idcontexts_%%
			  WHERE translation_unit_id = @id

		  SELECT translation_unit_id, alignment_data, align_model_date, insert_date
			FROM translation_unit_alignment_data_%% (nolock)
			WHERE translation_unit_id = @id
GO

CREATE PROCEDURE dbo.get_tu_count_%% AS
	SET NOCOUNT ON
	SELECT COUNT(*) FROM dbo.translation_units_%% (nolock)
GO

CREATE PROCEDURE dbo.update_tu_%% @id INT, @chd DATETIME, @chu NVARCHAR(50), 
	@lud DATETIME, @luu NVARCHAR(50), @usc INT,  @format INT, @origin INT, @confirmationLevel INT AS
	UPDATE dbo.translation_units_%% 
	SET change_date = @chd, change_user = @chu, last_used_date = @lud, 	last_used_user = @luu, usage_counter = @usc, 
										format = @format, origin = @origin, confirmationLevel = @confirmationLevel   
	WHERE id = @id
GO

CREATE PROCEDURE dbo.update_tu_data_batch_%%
	@serialization_version INT,
	@insert_date DATETIME,
	@sig_hash BIGINT,
	@cm_is_preceding_following BIT,
	@updateTuData as UpdateTuData2 readonly,
	@fuzzyData1 as FuzzyIndexData readonly,
	@fuzzyData2 as FuzzyIndexData readonly,
	@fuzzyData4 as FuzzyIndexData readonly,
	@fuzzyData8 as FuzzyIndexData readonly

AS
	CREATE TABLE #ExistingFuzzyData (id INT, feature INT)
	CREATE TABLE #NewFuzzyData (id INT, feature INT)
	CREATE TABLE #FuzzyDataToDelete (id INT, feature INT)

	CREATE TABLE #TusWithChangedHashes (id INT, oldsourcehash BIGINT, oldtargethash BIGINT, newsourcehash BIGINT, newtargethash BIGINT)
	CREATE TABLE #TusWithChangedSourceHashes (id INT, oldsourcehash BIGINT, oldtargethash BIGINT, newsourcehash BIGINT, newtargethash BIGINT)
	CREATE TABLE #TusWithChangedTargetHashes (id INT, oldsourcehash BIGINT, oldtargethash BIGINT, newsourcehash BIGINT, newtargethash BIGINT)
	
	-- need to retrieve existing segment hashes to know IF different, to wipe FGA (see LCC-3600) AND update CM values
	INSERT INTO #TusWithChangedHashes
	select existingtus.id, existingtus.source_hash, existingtus.target_hash, [@updateTuData].source_hash, [@updateTuData].target_hash
		FROM translation_units_%% existingtus 
		INNER JOIN @updateTuData 
		ON existingtus.id = [@updateTuData].id
		AND (existingtus.source_hash <> [@updateTuData].source_hash OR existingtus.target_hash <> [@updateTuData].target_hash)

	INSERT INTO #TusWithChangedSourceHashes
		SELECT id, oldsourcehash, oldtargethash, newsourcehash, newtargethash FROM #TusWithChangedHashes
			WHERE oldsourcehash <> newsourcehash
	IF @cm_is_preceding_following = 0
		INSERT INTO #TusWithChangedTargetHashes
				SELECT id, oldsourcehash, oldtargethash, newsourcehash, newtargethash FROM #TusWithChangedHashes
					WHERE oldtargethash <> newtargethash


	UPDATE dbo.translation_unit_alignment_data_%% with(ROWLOCK) 
		SET alignment_data = null,
			align_model_date = null,
			insert_date = @insert_date
		WHERE
			translation_unit_id IN (select id from #TusWithChangedHashes)
	DELETE FROM translation_unit_fragments_%% where translation_unit_id IN (select id from #TusWithChangedHashes)

	UPDATE tuTable
	SET tuTable.source_hash = [@updateTuData].source_hash, 
		tuTable.source_segment = [@updateTuData].source_text, 
		tuTable.target_hash = [@updateTuData].target_hash, 
		tuTable.target_segment = [@updateTuData].target_text,
		tuTable.source_token_data = [@updateTuData].source_token_data,
		tuTable.target_token_data = [@updateTuData].target_token_data,
		tuTable.tokenization_sig_hash = @sig_hash,
		tuTable.serialization_version = @serialization_version,
		tuTable.source_tags = [@updateTuData].source_tags,
		tuTable.target_tags = [@updateTuData].target_tags
	FROM translation_units_%% as tuTable
		INNER JOIN @updateTuData
			ON tuTable.id = [@updateTuData].id

	INSERT INTO translation_unit_fragments_%% (translation_unit_id, fragment_hash) 
		SELECT id, relaxed_hash FROM @updateTuData
		WHERE NOT EXISTS (SELECT 1 FROM translation_unit_fragments_%% WHERE translation_unit_id = id 
							AND translation_unit_fragments_%%.fragment_hash = [@updateTuData].relaxed_hash)

	-- in this batch, the same oldhash value may occur more than once, with
	-- different newhash values (i.e. tokenization has becoming more discriminating)
	-- but for a given oldhash value, we need the same newhash to be applied
	-- to all existing entries (see CM expansion docs)
	CREATE TABLE #HashChanges (oldhash BIGINT, newhash BIGINT);
	with cte as 
	(
		select *, Row_number() over(partition by oldsourcehash order by id) as rn
		from #TusWithChangedSourceHashes
	)
	insert into #HashChanges select oldsourcehash, newsourcehash from cte where rn = 1

	UPDATE contextsTable
	SET contextsTable.context1 = #HashChanges.newhash
	FROM translation_unit_contexts_%% contextsTable
		INNER JOIN #HashChanges
		ON contextsTable.context1 = #HashChanges.oldhash
	IF @cm_is_preceding_following = 1 
		UPDATE contextsTable
		SET contextsTable.context2 = #HashChanges.newhash
		FROM translation_unit_contexts_%% contextsTable
		INNER JOIN #HashChanges
			ON contextsTable.context2 = #HashChanges.oldhash

	IF @cm_is_preceding_following = 0
	BEGIN
		DELETE FROM #HashChanges;
		with cte2 as 
		(
			select *, Row_number() over(partition by oldtargethash order by id) as rn
			from #TusWithChangedTargetHashes
		)
		insert into #HashChanges select oldtargethash, newtargethash from cte2 where rn = 1

		UPDATE contextsTable
		SET contextsTable.context2 = #HashChanges.newhash
		FROM translation_unit_contexts_%% contextsTable
			INNER JOIN #HashChanges
			ON contextsTable.context2 = #HashChanges.oldhash
	END

	-- index tables need to know total features per TU, for scoring
	CREATE TABLE #fuzzyDataEx (total_features INT, id INT)

	-- find index entries that are already there; no need to add (but length value may have changed)
	INSERT INTO #ExistingFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData1 d INNER JOIN dbo.fuzzy_index1_%% i
			ON i.translation_unit_id = d.translation_unit_id AND i.feature = d.feature
	-- find index entries that need adding
	INSERT INTO #NewFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData1 d LEFT OUTER JOIN #ExistingFuzzyData i
			ON i.id = d.translation_unit_id AND i.feature = d.feature
			WHERE i.feature IS NULL

	-- find index entries that need deleting
	INSERT INTO #FuzzyDataToDelete
		SELECT i.translation_unit_id, i.feature FROM dbo.fuzzy_index1_%% i
			LEFT OUTER JOIN @fuzzyData1 d ON  d.translation_unit_id = i.translation_unit_id AND d.feature = i.feature
			WHERE i.translation_unit_id IN (SELECT id FROM @updateTuData) AND d.feature IS NULL

	-- delete them
	DELETE i
	FROM dbo.fuzzy_index1_%% i
		INNER JOIN #FuzzyDataToDelete d
		ON i.translation_unit_id = d.id AND i.feature = d.feature

	-- insert any new index entries
	INSERT INTO #fuzzyDataEx SELECT count(*), translation_unit_id FROM @fuzzyData1 group by translation_unit_id
	INSERT INTO dbo.fuzzy_index1_%%(feature, length, translation_unit_id) 
		SELECT feature, total_features, #NewFuzzyData.id FROM #NewFuzzyData INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = #NewFuzzyData.id
	
	-- update lengths where needed
	UPDATE i SET length = total_features
	FROM
		dbo.fuzzy_index1_%% i
		INNER JOIN #ExistingFuzzyData d ON i.translation_unit_id = d.id
		AND i.feature = d.feature
		INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = d.id
	WHERE length <> total_features

	--
	DELETE FROM #ExistingFuzzyData
	DELETE FROM #NewFuzzyData
	DELETE FROM #FuzzyDataToDelete

	INSERT INTO #ExistingFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData2 d INNER JOIN dbo.fuzzy_index2_%% i
			ON i.translation_unit_id = d.translation_unit_id AND i.feature = d.feature
	INSERT INTO #NewFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData2 d LEFT OUTER JOIN #ExistingFuzzyData i
			ON i.id = d.translation_unit_id AND i.feature = d.feature
			WHERE i.feature IS NULL

	INSERT INTO #FuzzyDataToDelete
		SELECT i.translation_unit_id, i.feature FROM dbo.fuzzy_index2_%% i
			LEFT OUTER JOIN @fuzzyData2 d ON  d.translation_unit_id = i.translation_unit_id AND d.feature = i.feature
			WHERE i.translation_unit_id IN (SELECT id FROM @updateTuData) AND d.feature IS NULL

	DELETE i
	FROM dbo.fuzzy_index2_%% i
		INNER JOIN #FuzzyDataToDelete d
		ON i.translation_unit_id = d.id AND i.feature = d.feature

	DELETE FROM #fuzzyDataEx
	INSERT INTO #fuzzyDataEx SELECT count(*), translation_unit_id FROM @fuzzyData2 group by translation_unit_id
	INSERT INTO dbo.fuzzy_index2_%%(feature, length, translation_unit_id) 
		SELECT feature, total_features, #NewFuzzyData.id FROM #NewFuzzyData INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = #NewFuzzyData.id

	UPDATE  i SET length = total_features
	FROM
		dbo.fuzzy_index2_%% i
		INNER JOIN #ExistingFuzzyData d ON i.translation_unit_id = d.id
		AND i.feature = d.feature
		INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = d.id
	WHERE length <> total_features

	--
	DELETE FROM #ExistingFuzzyData
	DELETE FROM #NewFuzzyData
	DELETE FROM #FuzzyDataToDelete

	INSERT INTO #ExistingFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData4 d INNER JOIN dbo.fuzzy_index4_%% i
			ON i.translation_unit_id = d.translation_unit_id AND i.feature = d.feature
	INSERT INTO #NewFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData4 d LEFT OUTER JOIN #ExistingFuzzyData i
			ON i.id = d.translation_unit_id AND i.feature = d.feature
			WHERE i.feature IS NULL

	INSERT INTO #FuzzyDataToDelete
		SELECT i.translation_unit_id, i.feature FROM dbo.fuzzy_index4_%% i
			LEFT OUTER JOIN @fuzzyData4 d ON  d.translation_unit_id = i.translation_unit_id AND d.feature = i.feature
			WHERE i.translation_unit_id IN (SELECT id FROM @updateTuData) AND d.feature IS NULL

	DELETE i
	FROM dbo.fuzzy_index4_%% i
		INNER JOIN #FuzzyDataToDelete d
		ON i.translation_unit_id = d.id AND i.feature = d.feature

	DELETE FROM #fuzzyDataEx
	INSERT INTO #fuzzyDataEx SELECT count(*), translation_unit_id FROM @fuzzyData4 group by translation_unit_id
	INSERT INTO dbo.fuzzy_index4_%%(feature, length, translation_unit_id) 
		SELECT feature, total_features, #NewFuzzyData.id FROM #NewFuzzyData INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = #NewFuzzyData.id

	UPDATE  i SET length = total_features
	FROM
		dbo.fuzzy_index4_%% i
		INNER JOIN #ExistingFuzzyData d ON i.translation_unit_id = d.id
		AND i.feature = d.feature
		INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = d.id
	WHERE length <> total_features

	--
	DELETE FROM #ExistingFuzzyData
	DELETE FROM #NewFuzzyData
	DELETE FROM #FuzzyDataToDelete

	INSERT INTO #ExistingFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData8 d INNER JOIN dbo.fuzzy_index8_%% i
			ON i.translation_unit_id = d.translation_unit_id AND i.feature = d.feature
	INSERT INTO #NewFuzzyData
		SELECT d.translation_unit_id, d.feature FROM @fuzzyData8 d LEFT OUTER JOIN #ExistingFuzzyData i
			ON i.id = d.translation_unit_id AND i.feature = d.feature
			WHERE i.feature IS NULL

	INSERT INTO #FuzzyDataToDelete
		SELECT i.translation_unit_id, i.feature FROM dbo.fuzzy_index8_%% i
			LEFT OUTER JOIN @fuzzyData8 d ON  d.translation_unit_id = i.translation_unit_id AND d.feature = i.feature
			WHERE i.translation_unit_id IN (SELECT id FROM @updateTuData) AND d.feature IS NULL

	DELETE i
	FROM dbo.fuzzy_index8_%% i
		INNER JOIN #FuzzyDataToDelete d
		ON i.translation_unit_id = d.id AND i.feature = d.feature

	DELETE FROM #fuzzyDataEx
	INSERT INTO #fuzzyDataEx SELECT count(*), translation_unit_id FROM @fuzzyData8 group by translation_unit_id
	INSERT INTO dbo.fuzzy_index8_%%(feature, length, translation_unit_id) 
		SELECT feature, total_features, #NewFuzzyData.id FROM #NewFuzzyData INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = #NewFuzzyData.id

	UPDATE i SET length = total_features
	FROM
		dbo.fuzzy_index8_%% i
		INNER JOIN #ExistingFuzzyData d ON i.translation_unit_id = d.id
		AND i.feature = d.feature
		INNER JOIN #fuzzyDataEx ON #fuzzyDataEx.id = d.id
	WHERE length <> total_features

GO

-- Updates the TU's segments AND index data, but leaves all field values constant
CREATE PROCEDURE dbo.update_tu_data_%% 
	@id INT, 
	@source_hash BIGINT, 
	@source_text NTEXT, 
	@target_hash BIGINT, 
	@target_text NTEXT,
	@data1 VARBINARY(MAX) = NULL, 
	@data2 VARBINARY(MAX) = NULL, 
	@data4 VARBINARY(MAX) = NULL,
	@data8 VARBINARY(MAX) = NULL,
	@source_token_data VARBINARY(MAX) = NULL,
	@target_token_data VARBINARY(MAX) = NULL,
	@insert_date DATETIME = NULL,
	@sig_hash BIGINT,
	@relaxed_hash BIGINT,
	@serialization_version INT,
	@source_tags VARBINARY(MAX) = NULL,
	@target_tags VARBINARY(MAX) = NULL,
	@cm_is_preceding_following BIT
AS
	SET NOCOUNT OFF
	-- delete existing fuzzy index data
	DELETE FROM dbo.fuzzy_index1_%% WHERE translation_unit_id = @id
	DELETE FROM dbo.fuzzy_index2_%% WHERE translation_unit_id = @id
	DELETE FROM dbo.fuzzy_index4_%% WHERE translation_unit_id = @id
	DELETE FROM dbo.fuzzy_index8_%% WHERE translation_unit_id = @id
	
	DECLARE @oldSourceHash BIGINT
	DECLARE @oldTargetHash BIGINT

	-- need to retrieve existing segment hashes to know IF different, to wipe FGA (see LCC-3600) AND update CM values
	SELECT @oldSourceHash = source_hash, @oldTargetHash = target_hash FROM dbo.translation_units_%% WHERE id = @id

	-- wipe FGA data if content has changed (see LCC-3600)
	IF @oldSourceHash <> @source_hash OR @oldTargetHash <> @target_hash
	BEGIN
		UPDATE dbo.translation_unit_alignment_data_%% with(ROWLOCK) 
		SET alignment_data = null,
			align_model_date = null,
			insert_date = @insert_date
		WHERE
			translation_unit_id = @id
		BEGIN
			DELETE FROM translation_unit_fragments_%% where translation_unit_id = @id
		END
	END
	UPDATE dbo.translation_units_%% 
	SET source_hash = @source_hash, 
		source_segment = @source_text, 
		target_hash = @target_hash, 
		target_segment = @target_text,
		source_token_data = @source_token_data,
		target_token_data = @target_token_data,
		tokenization_sig_hash = @sig_hash,
		serialization_version = @serialization_version,
		source_tags = @source_tags,
		target_tags = @target_tags
	WHERE 
		id = @id		

	IF NOT EXISTS (SELECT 1 FROM translation_unit_fragments_%% where translation_unit_id=@id AND fragment_hash=@relaxed_hash)
		INSERT INTO translation_unit_fragments_%% (translation_unit_id, fragment_hash) VALUES (@id, @relaxed_hash)


	IF @oldSourceHash <> @source_hash
	BEGIN
		UPDATE translation_unit_contexts_%% SET context1 = @source_hash WHERE context1 = @oldSourceHash
		IF @cm_is_preceding_following = 1 
			UPDATE translation_unit_contexts_%% SET context2 = @source_hash WHERE context2 = @oldSourceHash
	END

	IF @oldTargetHash <> @target_hash AND @cm_is_preceding_following = 0
		UPDATE translation_unit_contexts_%% SET context2 = @target_hash WHERE context2 = @oldTargetHash
	
	
	IF @data1 IS NOT NULL
		exec dbo.add_features_%% @id, @data1, 1
	IF @data2 IS NOT NULL
		exec dbo.add_features_%% @id, @data2, 2
	IF @data4 IS NOT NULL
		exec dbo.add_features_%% @id, @data4, 4
	IF @data8 IS NOT NULL
		exec dbo.add_features_%% @id, @data8, 8

	
GO

CREATE PROCEDURE dbo.delete_orphan_contexts_%% @cm_is_preceding_following BIT AS

	IF @cm_is_preceding_following = 0
		DELETE FROM translation_unit_contexts_%% WHERE translation_unit_contexts_%%.context1 <> 0 AND NOT EXISTS 
			(SELECT id FROM translation_units_%% WHERE (source_hash = translation_unit_contexts_%%.context1) 
				AND (target_hash = translation_unit_contexts_%%.context2))
	ELSE
		DELETE FROM translation_unit_contexts_%% WHERE
                (translation_unit_contexts_%%.context1 <> 0 AND NOT EXISTS 
					(SELECT id FROM translation_units_%% WHERE source_hash = translation_unit_contexts_%%.context1)) 
				OR (translation_unit_contexts_%%.context2 <> 0 AND NOT EXISTS 
					(SELECT id FROM translation_units_%% WHERE source_hash = translation_unit_contexts_%%.context2))
	;
	-- also important to remove duplicates, after a tokenization change to a less-discriminating tokenization
	with cte as 
	(
		select *, Row_number() over(partition by translation_unit_id, context1, context2 order by translation_unit_id, context1, context2) as rn
		from translation_unit_contexts_%%
	)
	delete from cte where rn <> 1

GO

CREATE PROCEDURE dbo.delete_tu_%% @id INT = NULL AS
	DECLARE @oldSourceHash BIGINT
	DECLARE @oldTargetHash BIGINT
	DECLARE @useCount BIGINT

	IF @id IS NOT NULL
		BEGIN
			SET NOCOUNT ON
			DELETE FROM dbo.fuzzy_index1_%% WHERE translation_unit_id = @id
			DELETE FROM dbo.fuzzy_index2_%% WHERE translation_unit_id = @id
			DELETE FROM dbo.fuzzy_index4_%% WHERE translation_unit_id = @id
			DELETE FROM dbo.fuzzy_index8_%% WHERE translation_unit_id = @id
			SET NOCOUNT OFF
			DELETE FROM dbo.translation_units_%% WHERE id = @id;		
		END
	ELSE 
		BEGIN
		DELETE FROM dbo.translation_units_%% 
		SET NOCOUNT ON
		DELETE FROM dbo.fuzzy_index1_%%
		DELETE FROM dbo.ff1_%%
		DELETE FROM dbo.fuzzy_index2_%%
		DELETE FROM dbo.ff2_%%
		DELETE FROM dbo.fuzzy_index4_%%
		DELETE FROM dbo.ff4_%%
		DELETE FROM dbo.fuzzy_index8_%%
		DELETE FROM dbo.ff8_%%
		UPDATE dbo.translation_memories 
			SET last_recompute_date = NULL, last_recompute_size = NULL 
			WHERE id = %%
		END
GO

CREATE PROCEDURE dbo.delete_tus_%% 	@tu_ids as TuIds readonly, @cm_is_preceding_following BIT, @delete_orphan_contexts BIT as
BEGIN
	DECLARE @SourceTargetHashesDeletedTable TABLE(source_hash_temp BIGINT, target_hash_temp BIGINT)
	DECLARE @SourceTargetHashesNotFoundTable TABLE(source_hash_temp2 BIGINT, target_hash_temp2 BIGINT)
	DECLARE @SourceHashesDeletedTable TABLE(source_hash_temp BIGINT)
	DECLARE @SourceHashesNotFoundTable TABLE(source_hash_temp2 BIGINT)
		
	SET NOCOUNT ON
	IF @delete_orphan_contexts = 1 
	BEGIN
		IF @cm_is_preceding_following = 0
			BEGIN
				INSERT INTO @SourceTargetHashesDeletedTable SELECT source_hash, target_hash FROM translation_units_%% WHERE id IN (SELECT TuId FROM @tu_ids)
			END
		ELSE
			BEGIN
				INSERT INTO @SourceHashesDeletedTable SELECT source_hash FROM translation_units_%% WHERE id IN (SELECT TuId FROM @tu_ids)
			END
	END

	DELETE FROM dbo.translation_unit_contexts_%%  WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.translation_unit_idcontexts_%% WHERE translation_unit_id in (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.translation_unit_alignment_data_%%  where translation_unit_id in (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index1_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index2_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index4_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index8_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	SET NOCOUNT OFF
	DELETE FROM translation_units_%% OUTPUT DELETED.id, Deleted.guid where id IN (SELECT TuId FROM @tu_ids)
	

	SET NOCOUNT ON
	-- delete any CM entries with 'orphan' context values
	IF @delete_orphan_contexts = 1 
	BEGIN
		IF @cm_is_preceding_following = 0
			BEGIN
				-- make a table of all the {source hash, target hash} pairs that just got deleted where there is no other TU with the same hash pair
				INSERT INTO @SourceTargetHashesNotFoundTable
					SELECT source_hash_temp, target_hash_temp FROM @SourceTargetHashesDeletedTable
						WHERE NOT EXISTS 
							(SELECT id FROM translation_units_%% WHERE source_hash = source_hash_temp AND target_hash = target_hash_temp )

				-- delete any contexts tha refer to such a pair
				DELETE FROM translation_unit_contexts_%%
					WHERE EXISTS (SELECT source_hash_temp2 FROM @SourceTargetHashesNotFoundTable WHERE source_hash_temp2 = context1 AND target_hash_temp2 = context2)
			END
		ELSE
			BEGIN
				-- make a table of all the source_hash values that just got deleted where there is no other TU with that source_hash
				INSERT INTO @SourceHashesNotFoundTable 
					SELECT source_hash_temp FROM @SourceHashesDeletedTable 
						WHERE NOT EXISTS 
							(SELECT id FROM translation_units_%% WHERE source_hash = source_hash_temp)

				-- delete any contexts referring to such a hash
				DELETE FROM translation_unit_contexts_%% WHERE context1 IN (SELECT source_hash_temp2 FROM @SourceHashesNotFoundTable)
					OR context2 IN (SELECT source_hash_temp2 FROM @SourceHashesNotFoundTable)

			END
	END
	SET NOCOUNT OFF


	
END
GO

CREATE PROCEDURE dbo.delete_tus_filtered_%% 
	@start_after INT, 
	@count INT, 
	@forward INT,
    @filterParam as TUFilterParams readonly, @cm_is_preceding_following BIT, @delete_orphan_contexts BIT as
BEGIN
	DECLARE @SourceTargetHashesDeletedTable TABLE(source_hash_temp BIGINT, target_hash_temp BIGINT)
	DECLARE @SourceTargetHashesNotFoundTable TABLE(source_hash_temp2 BIGINT, target_hash_temp2 BIGINT)
	DECLARE @SourceHashesDeletedTable TABLE(source_hash_temp BIGINT)
	DECLARE @SourceHashesNotFoundTable TABLE(source_hash_temp2 BIGINT)

	SET NOCOUNT ON
	
	declare @tu_ids as TuIDs  

	declare @filterParamCount int, @applyHardFilter bit
	select @filterParamCount = count(*) from @filterParam
	
	SET @applyHardFilter = CASE 
                 WHEN ( @filterParamCount > 0 ) THEN 1
                 ELSE 0
              END
	if (@applyHardFilter = 1)
		insert into @tu_ids	
		exec dbo.getFilteredTus_%% 1, @start_after, @count, @forward, @filterParam	
	else
		insert into @tu_ids
		exec dbo.get_tu_ids_simple_%% @start_after, @count, @forward
		
	SET NOCOUNT ON
	IF @delete_orphan_contexts = 1 
	BEGIN
		IF @cm_is_preceding_following = 0
			BEGIN
				INSERT INTO @SourceTargetHashesDeletedTable SELECT source_hash, target_hash FROM translation_units_%% WHERE id IN (SELECT TuId FROM @tu_ids)
			END
		ELSE
			BEGIN
				INSERT INTO @SourceHashesDeletedTable SELECT source_hash FROM translation_units_%% WHERE id IN (SELECT TuId FROM @tu_ids)
			END
	END

	DELETE FROM dbo.translation_unit_contexts_%%  WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.translation_unit_idcontexts_%% WHERE translation_unit_id in (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.translation_unit_alignment_data_%%  where translation_unit_id in (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index1_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index2_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index4_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	DELETE FROM dbo.fuzzy_index8_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
	SET NOCOUNT OFF
	DELETE FROM translation_units_%% OUTPUT DELETED.id, Deleted.guid where id IN (SELECT TuId FROM @tu_ids)
	

	SET NOCOUNT ON
	-- delete any CM entries with 'orphan' context values
	IF @delete_orphan_contexts = 1 
	BEGIN
		IF @cm_is_preceding_following = 0
			BEGIN
				-- make a table of all the {source hash, target hash} pairs that just got deleted where there is no other TU with the same hash pair
				INSERT INTO @SourceTargetHashesNotFoundTable
					SELECT source_hash_temp, target_hash_temp FROM @SourceTargetHashesDeletedTable
						WHERE NOT EXISTS 
							(SELECT id FROM translation_units_%% WHERE source_hash = source_hash_temp AND target_hash = target_hash_temp )

				-- delete any contexts tha refer to such a pair
				DELETE FROM translation_unit_contexts_%%
					WHERE EXISTS (SELECT source_hash_temp2 FROM @SourceTargetHashesNotFoundTable WHERE source_hash_temp2 = context1 AND target_hash_temp2 = context2)
			END
		ELSE
			BEGIN
				-- make a table of all the source_hash values that just got deleted where there is no other TU with that source_hash
				INSERT INTO @SourceHashesNotFoundTable 
					SELECT source_hash_temp FROM @SourceHashesDeletedTable 
						WHERE NOT EXISTS 
							(SELECT id FROM translation_units_%% WHERE source_hash = source_hash_temp)

				-- delete any contexts referring to such a hash
				DELETE FROM translation_unit_contexts_%% WHERE context1 IN (SELECT source_hash_temp2 FROM @SourceHashesNotFoundTable)
					OR context2 IN (SELECT source_hash_temp2 FROM @SourceHashesNotFoundTable)

			END
	END
	SET NOCOUNT OFF


	
END
GO

CREATE PROCEDURE dbo.add_attribute_value_%% @attribute_id INT, @tu_id INT, @date_value DATETIME = NULL, 
	@string_value NVARCHAR(MAX) = NULL,	@numeric_value INT = NULL, @picklist_value_id INT = NULL AS
	IF @date_value IS NOT NULL
		INSERT INTO dbo.date_attributes_%%(attribute_id, value, translation_unit_id) 
			VALUES(@attribute_id, @date_value, @tu_id)
	ELSE IF @string_value IS NOT NULL
		INSERT INTO dbo.string_attributes_%%(attribute_id, value, translation_unit_id) 
			VALUES(@attribute_id, @string_value, @tu_id)
	ELSE IF @numeric_value IS NOT NULL
		INSERT INTO dbo.numeric_attributes_%%(attribute_id, value, translation_unit_id) 
			VALUES(@attribute_id, @numeric_value, @tu_id)
	ELSE IF @picklist_value_id IS NOT NULL
		INSERT INTO dbo.picklist_attributes_%%(translation_unit_id, picklist_value_id)
			VALUES(@tu_id, @picklist_value_id)
GO

CREATE PROCEDURE dbo.delete_attribute_values_%% @tu_id INT AS
	DELETE FROM dbo.date_attributes_%% WHERE translation_unit_id = @tu_id
	DELETE FROM dbo.string_attributes_%% WHERE translation_unit_id = @tu_id
	DELETE FROM dbo.numeric_attributes_%% WHERE translation_unit_id = @tu_id
	DELETE FROM dbo.picklist_attributes_%% WHERE translation_unit_id = @tu_id
GO

CREATE PROCEDURE dbo.add_tu_context_%% @tu_id INT, @context1 BIGINT, @context2 BIGINT AS
	IF NOT EXISTS (SELECT translation_unit_id FROM dbo.translation_unit_contexts_%% WHERE translation_unit_id = @tu_id 
		AND context1 = @context1 AND context2 = @context2)
		INSERT INTO dbo.translation_unit_contexts_%%(translation_unit_id, context1, context2) 
		VALUES(@tu_id, @context1, @context2)
GO

CREATE PROCEDURE dbo.get_tu_contexts_%% @tu_id INT AS
	SET NOCOUNT ON
	SELECT context1, context2 
		FROM dbo.translation_unit_contexts_%% 
		WHERE translation_unit_id = @tu_id
GO

CREATE PROCEDURE dbo.get_tus_contexts_%% @from_tu_id INT, @into_tu_id INT AS
	SET NOCOUNT ON
	SELECT 
		translation_unit_id, context1, context2 
	FROM 
		dbo.translation_unit_contexts_%% 
	WHERE 
		translation_unit_id >= @from_tu_id AND translation_unit_id <= @into_tu_id
	ORDER BY
		translation_unit_id ASC
GO

CREATE PROCEDURE dbo.get_tu_idcontexts_%% @tu_id INT AS
	SET NOCOUNT ON
	SELECT idcontext
		FROM dbo.translation_unit_idcontexts_%% 
		WHERE translation_unit_id = @tu_id
GO

CREATE PROCEDURE dbo.get_tus_idcontexts_%% @from_tu_id INT, @into_tu_id INT AS
	SET NOCOUNT ON
	SELECT 
		translation_unit_id, idcontext
	FROM 
		dbo.translation_unit_idcontexts_%% 
	WHERE 
		translation_unit_id >= @from_tu_id AND translation_unit_id <= @into_tu_id
	ORDER BY
		translation_unit_id ASC
GO

CREATE PROCEDURE dbo.delete_tu_contexts_%% @tu_id INT AS
	DELETE FROM dbo.translation_unit_contexts_%% WHERE translation_unit_id = @tu_id
	DELETE FROM dbo.translation_unit_idcontexts_%% WHERE translation_unit_id = @tu_id
GO

CREATE PROCEDURE dbo.get_tu_ids_%% @start_after INT, @count INT, @forward INT AS
	SET NOCOUNT ON
	IF @forward > 0
		SELECT TOP (@count) id, guid
			FROM dbo.translation_units_%% WHERE id > @start_after
			ORDER BY id ASC
	ELSE		
		SELECT TOP (@count) id, guid
			FROM dbo.translation_units_%% WHERE id <= @start_after
			ORDER BY id DESC
GO

CREATE PROCEDURE dbo.get_tu_ids_simple_%% @start_after INT, @count INT, @forward INT AS
	SET NOCOUNT ON
	IF @forward > 0
		SELECT TOP (@count) id
			FROM dbo.translation_units_%% WHERE id > @start_after
			ORDER BY id ASC
	ELSE		
		SELECT TOP (@count) id
			FROM dbo.translation_units_%% WHERE id <= @start_after
			ORDER BY id DESC
GO

CREATE PROCEDURE dbo.get_tus_%% @start_after INT, @count INT, @forward INT AS
	SET NOCOUNT ON
	IF @forward > 0
		SELECT TOP (@count) id
			FROM dbo.translation_units_%% WHERE id > @start_after
			ORDER BY id ASC
	ELSE		
		SELECT TOP (@count) id
			FROM dbo.translation_units_%% WHERE id <= @start_after
			ORDER BY id DESC
GO

CREATE PROCEDURE dbo.get_tus_ex_%% 
	@start_after INT, 
	@count INT, 
	@forward INT ,
	@returnIdContext INT,
	@includeContextContent INT,
	@cm_is_preceding_following BIT
AS
	SET NOCOUNT ON
	
	DECLARE @ResultIDs TABLE(id INT NOT NULL PRIMARY KEY)
	
	IF @forward > 0
		INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
			FROM dbo.translation_units_%% 
			WHERE id > @start_after
			ORDER BY id ASC
	ELSE
		INSERT INTO @ResultIDs
			SELECT TOP (@count) id
			FROM dbo.translation_units_%% 
			WHERE id <= @start_after
			ORDER BY id DESC

	SELECT id, guid, %%, source_hash, source_segment, 
		target_hash, target_segment, 
		creation_date, creation_user, change_date, change_user, 
		last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags,  format, origin, confirmationLevel
	FROM dbo.translation_units_%% 
	WHERE id IN (SELECT id FROM @ResultIDs)
	
	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
		FROM dbo.date_attributes_%% da INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 	
		WHERE da.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY da.translation_unit_id, da.attribute_id		
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
		FROM dbo.string_attributes_%% sa INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id 		
		WHERE sa.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
	FROM dbo.numeric_attributes_%% na  INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id 
		WHERE na.translation_unit_id IN (SELECT id FROM @ResultIDs) 

	ORDER BY na.translation_unit_id, na.attribute_id
	
	SELECT pa.translation_unit_id, pv.attribute_id,  a.name, a.type, pa.picklist_value_id 
	FROM dbo.picklist_attributes_%% pa 
		INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id		
		INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
		WHERE pa.translation_unit_id IN (SELECT id FROM @ResultIDs) 
	ORDER BY pa.translation_unit_id, pv.attribute_id

	IF @includeContextContent = 1
		IF @cm_is_preceding_following = 1
			SELECT translation_unit_id, context1, context2, t1.source_segment, t1.source_tags, t1.source_token_data, t2.source_segment, t2.source_tags, t2.source_token_data, t1.serialization_version, t2.serialization_version
				FROM dbo.translation_unit_contexts_%% 
				LEFT OUTER JOIN translation_units_%% As t1
				ON t1.id = (select max(id) from translation_units_%% where source_hash = context1)
				LEFT OUTER JOIN translation_units_%% As t2
				ON t2.id = (select max(id) from translation_units_%% where source_hash = context2)
				WHERE	translation_unit_id IN (SELECT id FROM @ResultIDs) 
				ORDER BY translation_unit_id
		ELSE
			SELECT translation_unit_id, context1, context2, source_segment, source_tags, source_token_data, target_segment, target_tags, target_token_data, serialization_version
				FROM dbo.translation_unit_contexts_%% 
				LEFT OUTER JOIN translation_units_%%
				ON id = (select max(id) from translation_units_%% where source_hash = context1 and target_hash = context2)
				WHERE	translation_unit_id IN (SELECT id FROM @ResultIDs) 
				ORDER BY translation_unit_id
	ELSE
		SELECT translation_unit_id, context1, context2 
			FROM dbo.translation_unit_contexts_%% 
			WHERE	translation_unit_id IN (SELECT id FROM @ResultIDs) 
			ORDER BY translation_unit_id

	IF @returnIdContext = 1		
	SELECT translation_unit_id, idcontext
		FROM dbo.translation_unit_idcontexts_%% 
		WHERE	translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY translation_unit_id

	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM dbo.translation_unit_alignment_data_%% (nolock)
		WHERE	translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY translation_unit_id
GO

CREATE PROCEDURE dbo.add_tu_contexts_%% @tu_id INT, @data VARBINARY(MAX)
AS
	DECLARE @pos INT, @len INT
	DECLARE @sh BIGINT, @th BIGINT
	SET @len = DATALENGTH(@data)
	SET @pos = 1
	WHILE @pos < @len
	BEGIN
		SET @sh = CONVERT(BIGINT, SUBSTRING(@data, @pos, 8))
		SET @pos = @pos + 8
		SET @th = CONVERT(BIGINT, SUBSTRING(@data, @pos, 8))
		SET @pos = @pos + 8
		EXEC dbo.add_tu_context_%% @tu_id, @sh, @th
	END
GO

CREATE PROCEDURE dbo.add_tu_idcontexts_%% 
@tu_id INT,
@idcontexts as IdContexts readonly
AS
	INSERT INTO translation_unit_idcontexts_%%
	SELECT  @tu_id as ID, ctx.idcontext
	FROM @idcontexts ctx
	LEFT JOIN translation_unit_idcontexts_%% dbcontext
		ON ctx.idcontext = dbcontext.idcontext AND @tu_id = dbcontext.translation_unit_id
	WHERE dbcontext.translation_unit_id is NULL
GO

CREATE PROCEDURE dbo.delete_tu_fragments_%% @tu_ids as TuIds readonly
AS
	DELETE FROM dbo.translation_unit_fragments_%% WHERE translation_unit_id IN (SELECT TuId FROM @tu_ids)
GO

CREATE PROCEDURE dbo.insert_tu_fragments_%% @index_entries as TuFragmentIndexEntries readonly
AS
	INSERT INTO dbo.translation_unit_fragments_%%(translation_unit_id, fragment_hash) 
		SELECT TuId, FragmentHash FROM @index_entries
GO

CREATE PROCEDURE dbo.update_tu_alignmentdata_%% @tu_id INT, @insert_date DATETIME,  @data VARBINARY(MAX) = NULL, @align_model_date DATETIME = NULL
AS
	SET DEADLOCK_PRIORITY LOW;  
	
	UPDATE dbo.translation_unit_alignment_data_%% with(ROWLOCK) SET alignment_data = @data, align_model_date = @align_model_date
	WHERE translation_unit_id = @tu_id AND insert_date = @insert_date

	SET DEADLOCK_PRIORITY NORMAL;
GO

CREATE PROCEDURE dbo.get_reindex_required_%% @sig_hash BIGINT
AS
	SELECT TOP 1 1 
	FROM translation_units_%% (nolock) 
	WHERE tokenization_sig_hash is null 
		OR tokenization_sig_hash > @sig_hash
		OR tokenization_sig_hash < @sig_hash
GO

CREATE PROCEDURE dbo.get_tus_for_reindex_%% @start_after INT, @count INT, @sig_hash BIGINT
AS
	SELECT TOP(@count) id, guid, source_hash, source_segment, target_hash, target_segment, source_token_data, target_token_data, null, null, null
												, serialization_version, source_tags, target_tags
                                              FROM translation_units_%%
                                              WHERE id > @start_after AND (tokenization_sig_hash is null or tokenization_sig_hash <> @sig_hash)
                                              ORDER BY id ASC
GO

CREATE PROCEDURE dbo.get_dup_seg_hashes_%% @sig_hash BIGINT, @target BIT
AS
	IF @target = 1
		select id, target_hash, tokenization_sig_hash from translation_units_%% where target_hash in 
                    (select target_hash from translation_units_%%
                    group by target_hash
                    having count(*) >1) order by target_hash
	ELSE
		select id, source_hash, tokenization_sig_hash from translation_units_%% where source_hash in 
                    (select source_hash from translation_units_%%
                    group by source_hash
                    having count(*) >1) order by source_hash

GO

CREATE PROCEDURE dbo.create_hash_expansion_map_%% @tuIdsWithDupHashes AS GroupedTuIds readonly, @target BIT
AS
	DECLARE @startGroupId INT, @finishGroupId INT, @tuid INT, @lowestTuid INT, @newHash BIGINT, @hashCount INT
	CREATE TABLE #NewHashes (id INT, althash BIGINT)
	CREATE TABLE #UniqueNewHashes (althash BIGINT)
	CREATE TABLE #HashesToAdd (hash BIGINT)
	CREATE TABLE #IdsForThisGroup (IdInGroup INT)

	select @startGroupId = min(GroupId), @finishGroupId = max(GroupId) 
	from @tuIdsWithDupHashes

	-- process tuids in groups; the tus in each group formerly shared a segment hash
	while @startGroupId <= @finishGroupId
	BEGIN
		DELETE FROM #UniqueNewHashes
		DELETE FROM #NewHashes
		DELETE FROM #IdsForThisGroup

		INSERT INTO #IdsForThisGroup SELECT TuId FROM @tuIdsWithDupHashes WHERE GroupId = @startGroupId

		-- get the lowest tuid in this group; this will be the tu that was reindexed first
		-- so its new segment hash will have been used to update CM entries
		SELECT @lowestTuId = MIN(IdInGroup) FROM #IdsForThisGroup

		-- get all the new hash values for the TUs that shared this one previously
		-- (some may now still be duplicate)
		-- distinguishing the one that's already been used to update context entries
		IF @target = 1
		BEGIN
			INSERT INTO #NewHashes 
				SELECT id, target_hash FROM translation_units_%% INNER JOIN #IdsForThisGroup ON id = IdInGroup 
		END		
		ELSE
		BEGIN
			INSERT INTO #NewHashes 
				SELECT id, source_hash FROM translation_units_%% INNER JOIN #IdsForThisGroup ON id = IdInGroup 
		END		

		SELECT @newHash = althash FROM #NewHashes 
			WHERE id = @lowestTuId

		-- get the unique hash values
		-- but not necessarily all of them
		-- if a certain hash value was previously shared by hundreds of segments, it's
		-- unrealistic to expand them all (especially as context1 x context2 can then be a large number)
		-- see https://confluence.sdl.com/display/LCC/Limitations+of+CM+expansion
		INSERT INTO #UniqueNewHashes SELECT DISTINCT althash FROM #NewHashes WHERE althash <> @newHash
		IF @target = 1
		BEGIN
			INSERT INTO #TargetHashMap
				SELECT 
				TOP(10) 
				@newHash, althash FROM #UniqueNewHashes

		END
		ELSE
		BEGIN
			INSERT INTO #SourceHashMap
				SELECT 
				TOP(10) 
				@newHash, althash FROM #UniqueNewHashes
		END

		set @startGroupId = @startGroupId+1;
	END

GO

CREATE PROCEDURE dbo.process_additional_hashes_for_tu_%% @start_after INT, @count INT, @cm_is_preceding_following BIT
AS
	-- find max id for batch
	DECLARE @idtable table (batchid INT)
	INSERT INTO @idtable SELECT TOP (@count) id FROM translation_units_%% WHERE id > @start_after order by id
	DECLARE @maxid INT

	SELECT @maxid = max(batchid) FROM @idtable

	IF @cm_is_preceding_following = 1 
	BEGIN
		-- insert (if not exist) entries with alternative values for context1
		INSERT INTO dbo.translation_unit_contexts_%%
			SELECT tuc.translation_unit_id, c1.althash ctx1, context2 ctx2 FROM translation_unit_contexts_%% tuc
				INNER JOIN #SourceHashMap c1
				ON c1.newHash = context1
				AND tuc.translation_unit_id <= @maxid AND tuc.translation_unit_id > @start_after
			WHERE NOT EXISTS (SELECT 1 FROM translation_unit_contexts_%% tuc2 WHERE 
					tuc2.translation_unit_id = tuc.translation_unit_id AND tuc2.context1 = c1.althash and tuc2.context2 = context2)
		-- insert (if not exist) entries with alternative values for context2
		INSERT INTO dbo.translation_unit_contexts_%%
			SELECT tuc.translation_unit_id, context1 ctx1, c2.althash ctx2 FROM translation_unit_contexts_%% tuc
				INNER JOIN #SourceHashMap c2
				ON c2.newHash = context2
				AND tuc.translation_unit_id <= @maxid AND tuc.translation_unit_id > @start_after
			WHERE NOT EXISTS (SELECT 1 FROM translation_unit_contexts_%% tuc2 WHERE 
					tuc2.translation_unit_id = tuc.translation_unit_id AND tuc2.context1 = context1 and tuc2.context2 = c2.althash)

		-- insert (if not exist) entries where both context1 and context2 have alternative values
		INSERT INTO dbo.translation_unit_contexts_%%
			SELECT tuc.translation_unit_id, c1.althash ctx1, c2.althash ctx2 FROM translation_unit_contexts_%% tuc
				INNER JOIN #SourceHashMap c1
				ON c1.newHash = context1
				AND tuc.translation_unit_id <= @maxid AND tuc.translation_unit_id > @start_after
				INNER JOIN #SourceHashMap c2
				ON c2.newHash = context2
			WHERE NOT EXISTS (SELECT 1 FROM translation_unit_contexts_%% tuc2 WHERE 
					tuc2.translation_unit_id = tuc.translation_unit_id AND tuc2.context1 = c1.althash and tuc2.context2 = c2.althash)
	END		
	ELSE
	BEGIN
		-- case 1 - alternative values for context1, keep context2 the same
		INSERT INTO dbo.translation_unit_contexts_%%
			SELECT tuc.translation_unit_id, c1.althash ctx1, context2 ctx2 FROM translation_unit_contexts_%% tuc
				INNER JOIN #SourceHashMap c1
				ON c1.newHash = context1
				AND tuc.translation_unit_id <= @maxid AND tuc.translation_unit_id > @start_after
			WHERE NOT EXISTS (SELECT 1 FROM translation_unit_contexts_%% tuc2 WHERE 
					tuc2.translation_unit_id = tuc.translation_unit_id AND tuc2.context1 = c1.althash and tuc2.context2 = context2)
					AND EXISTS (SELECT 1 FROM translation_units_%% WHERE source_hash = c1.althash and target_hash = context2)

		-- case 2 - alternative values for context2, keep context1 the same
		INSERT INTO dbo.translation_unit_contexts_%%
			SELECT tuc.translation_unit_id, context1 ctx1, c2.althash ctx2 FROM translation_unit_contexts_%% tuc
				INNER JOIN #TargetHashMap c2
				ON c2.newHash = context2
				AND tuc.translation_unit_id <= @maxid AND tuc.translation_unit_id > @start_after
			WHERE NOT EXISTS (SELECT 1 FROM translation_unit_contexts_%% tuc2 WHERE 
					tuc2.translation_unit_id = tuc.translation_unit_id AND tuc2.context1 = context1 and tuc2.context2 = c2.althash)
					AND EXISTS (SELECT 1 FROM translation_units_%% WHERE source_hash = context1 and target_hash = c2.althash)

		-- case 3 - alternative values for context1 and context2
		INSERT INTO dbo.translation_unit_contexts_%%
			SELECT tuc.translation_unit_id, c1.althash ctx1, c2.althash ctx2 FROM translation_unit_contexts_%% tuc
				INNER JOIN #SourceHashMap c1
				ON c1.newHash = context1
				AND tuc.translation_unit_id <= @maxid AND tuc.translation_unit_id > @start_after
				INNER JOIN #TargetHashMap c2
				ON c2.newHash = context2
			WHERE NOT EXISTS (SELECT 1 FROM translation_unit_contexts_%% tuc2 WHERE 
					tuc2.translation_unit_id = tuc.translation_unit_id AND tuc2.context1 = c1.althash and tuc2.context2 = c2.althash)
					AND EXISTS (SELECT 1 FROM translation_units_%% WHERE source_hash = c1.althash and target_hash = c2.althash)
	END

	SELECT @maxid
GO



CREATE PROCEDURE dbo.get_tu_count_for_reindex_%% @sig_hash BIGINT
AS
	SELECT SUM(q.qty) FROM (
		SELECT COUNT(*) qty FROM [dbo].[translation_units_%%] (nolock) WHERE tokenization_sig_hash > @sig_hash or tokenization_sig_hash < @sig_hash
		UNION ALL
		SELECT COUNT(*) qty FROM [dbo].[translation_units_%%] (nolock) WHERE tokenization_sig_hash is null
	) q
GO

---- Fuzzy index
CREATE PROCEDURE dbo.add_features_%% @tu_id INT, @data VARBINARY(MAX), @type INT 
AS
	DECLARE @pos INT, @len INT, @feature INT
	SET @len = DATALENGTH(@data)
	SET @pos = 1
	WHILE @pos < @len
		BEGIN
		SET @feature = SUBSTRING(@data, @pos, 4)
		IF @type = 1
			BEGIN
			INSERT INTO dbo.fuzzy_index1_%%(feature, length, translation_unit_id) 
				VALUES(@feature, @len / 4, @tu_id)
			END
		ELSE IF @type = 2
			BEGIN
			INSERT INTO dbo.fuzzy_index2_%%(feature, length, translation_unit_id) 
				VALUES(@feature, @len / 4, @tu_id)
			END
		ELSE IF @type = 8
			BEGIN
			INSERT INTO dbo.fuzzy_index8_%%(feature, length, translation_unit_id) 
				VALUES(@feature, @len / 4, @tu_id)
			END
		ELSE IF @type = 4
			BEGIN
			INSERT INTO dbo.fuzzy_index4_%%(feature, length, translation_unit_id) 
				VALUES(@feature, @len / 4, @tu_id)
			END
		SET @pos = @pos + 4
		END

GO

CREATE PROCEDURE dbo.recompute_frequencies_%% AS
	DECLARE @top INT
	SET NOCOUNT ON
	SET @top = (SELECT value FROM [dbo].parameters WHERE translation_memory_id IS NULL AND name = 'FREQUENCYTOP')
	IF @top IS NULL SET @top = 1000
	
 DELETE FROM dbo.ff1_%%  
	INSERT INTO dbo.ff1_%% SELECT TOP (@top) feature, COUNT(*) frequency FROM dbo.fuzzy_index1_%% 
		GROUP BY feature ORDER BY frequency DESC
    
 DELETE FROM dbo.ff2_%%  
	INSERT INTO dbo.ff2_%% SELECT TOP (@top) feature, COUNT(*) frequency FROM dbo.fuzzy_index2_%%
		GROUP BY feature ORDER BY frequency DESC
    
 DELETE FROM dbo.ff4_%%  
	INSERT INTO dbo.ff4_%% SELECT TOP (@top) feature, COUNT(*) frequency FROM dbo.fuzzy_index4_%% 
		GROUP BY feature ORDER BY frequency DESC
    
 DELETE FROM dbo.ff8_%%  
	INSERT INTO dbo.ff8_%% SELECT TOP (@top) feature, COUNT(*) frequency FROM dbo.fuzzy_index8_%%
		GROUP BY feature ORDER BY frequency DESC
    
 UPDATE dbo.translation_memories SET last_recompute_date = GETUTCDATE(), last_recompute_size =   
  (SELECT COUNT(*) FROM dbo.translation_units_%%)  
  WHERE id = %% 

GO

CREATE PROCEDURE dbo.clear_fuzzy_index_%% 
	@type INT
AS
	IF @type = 1
		BEGIN
			DELETE FROM dbo.ff1_%%
			DELETE FROM dbo.fuzzy_index1_%%
		END
	ELSE IF @type = 2
		BEGIN
			DELETE FROM dbo.ff2_%%
			DELETE FROM dbo.fuzzy_index2_%%
		END
	ELSE IF @type = 4
		BEGIN
			DELETE FROM dbo.ff4_%%
			DELETE FROM dbo.fuzzy_index4_%%
		END
	ELSE IF @type = 8
		BEGIN
			DELETE FROM dbo.ff8_%%
			DELETE FROM dbo.fuzzy_index8_%%
		END

GO
  
CREATE PROCEDURE dbo.fuzzy_search_%% @features NVARCHAR(MAX), @length INT, @minScore INT, @count INT, @type INT, @last_id INT, @context1 BIGINT,
									@context2 BIGINT, @DescendingOrder BIT
	AS
	DECLARE @cmd NVARCHAR(MAX)
	SET NOCOUNT ON
	DECLARE @maxLength INT, @ignored INT, @minMatches INT, @newMinMatches INT, @minHaving INT
	SET @minHaving = (SELECT value FROM dbo.parameters WHERE translation_memory_id = %% AND name = 'MINHAVING' + LTRIM(STR(@type)))
	SET @minScore = @minScore + (SELECT value FROM dbo.parameters WHERE translation_memory_id = %% AND name = 'ADDTOMINSCORE')
	IF @minScore > 100 SET @minScore = 100
	SET @minMatches = (@minScore * @length + 99) / 100
	SET @maxLength = (@length * 100 + @minScore - 1) / @minScore
	SET @newMinMatches = @minMatches
	
	DECLARE @ResultTable TABLE(id INT PRIMARY KEY, [length] int, matches INT)
	CREATE TABLE #CountTable (feature INT PRIMARY KEY, occurences INT, ignored INT DEFAULT 0)

	DECLARE @s NVARCHAR(MAX), @dataLen INT, @pos INT, @w NVARCHAR(50), @found INT
	
	SET @dataLen = (DATALENGTH(@features) / 2) - 2
	SET @s = SUBSTRING(@features, 2, @dataLen)

	SET @pos = 1
	SET @found = CHARINDEX(N',', @s, @pos)
	WHILE @found > 0
	BEGIN
		SET @w = SUBSTRING(@s, @pos, @found - @pos)
		IF  @newMinMatches > @minHaving
			INSERT INTO #CountTable VALUES(@w, dbo.get_feature_frequency_%%(@w, @type), 0)
		else
			INSERT INTO #CountTable VALUES(@w, 0, 0)
	
		SET @pos = @found + 1
		SET @found = CHARINDEX(N',', @s, @pos)
	END
	
	SET @w = SUBSTRING(@s, @pos, @dataLen + 1 - @pos)
	
	IF  @newMinMatches > @minHaving
		INSERT INTO #CountTable VALUES(@w, dbo.get_feature_frequency_%%(@w, @type), 0)
	else
		INSERT INTO #CountTable VALUES(@w, 0, 0)
		
			
	IF @length > 1
	BEGIN
		DECLARE @occ INT
		
		WHILE @newMinMatches > @minHaving
		BEGIN
			SELECT @occ = (SELECT MAX(occurences) FROM #CountTable WHERE ignored = 0)
			IF @occ IS NULL OR @occ < 1000
				BREAK;

			UPDATE TOP(1) #CountTable 
				SET ignored = 1 
				WHERE occurences = @occ
			
			SET @newMinMatches = @newMinMatches - 1
		END
	END

	SELECT @ignored = COUNT(*) FROM #CountTable WHERE ignored = 1

	IF @type = 1	
			INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index1_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND fi.length BETWEEN @minMatches AND @maxLength 
				AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches
			
	ELSE IF @type = 2	
	
		INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index2_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches
			
	ELSE IF @type = 4	
	
		INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index4_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches
		
	ELSE IF @type = 8 	
		INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index8_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND fi.length BETWEEN @minMatches AND @maxLength 
			AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches

	IF @ignored > 0
	BEGIN

		DECLARE @resultTopMinMatches INT
		
		SELECT @resultTopMinMatches = MIN(matches)
		FROM @ResultTable
		WHERE id IN (SELECT TOP(@count) id FROM @ResultTable ORDER BY matches DESC)

		DELETE FROM @ResultTable
		WHERE 
			matches + @ignored < @minMatches
			OR matches + @ignored < @resultTopMinMatches
				
		IF @type = 1
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index1_%% fi WITH (INDEX = fi1_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
				WHERE c.ignored = 1)
		ELSE IF @type = 2
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index2_%% fi WITH (INDEX = fi2_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
                WHERE c.ignored = 1)
		ELSE IF @type = 8
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index8_%% fi WITH (INDEX = fi8_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
                WHERE c.ignored = 1)
		ELSE IF @type = 4
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index4_%% fi WITH (INDEX = fi4_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
                WHERE c.ignored = 1)
    END
	
	DECLARE @ResultIDs TABLE(id INT)
	IF @DescendingOrder = 1
	BEGIN
		INSERT INTO @ResultIDs 
		SELECT TOP (@count) rt.id 
		FROM @ResultTable rt
		WHERE rt.matches >= @minMatches 
		ORDER BY rt.matches DESC, rt.[length], rt.id desc
	END
	else
	BEGIN
		INSERT INTO @ResultIDs 
		SELECT TOP (@count) rt.id 
		FROM @ResultTable rt
		WHERE rt.matches >= @minMatches 
		ORDER BY rt.matches DESC, rt.[length], rt.id ASC
	END


	SELECT id, guid, 1, source_hash, source_segment,
		target_hash, target_segment,
		creation_date, creation_user, change_date, change_user, 
		last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% 
			WHERE id IN (SELECT id FROM @ResultIDs) 
	
	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value
		FROM dbo.date_attributes_%% da INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id
		WHERE da.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value
		FROM dbo.string_attributes_%% sa INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
		WHERE sa.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value
		FROM dbo.numeric_attributes_%% na INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
		WHERE na.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
		FROM dbo.picklist_attributes_%% pa 
			INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id	
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id	
		WHERE pa.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY pa.translation_unit_id, pv.attribute_id

	IF @context1 <> -1 
	BEGIN
		DECLARE @FullContextMatch TABLE(translation_unit_id INT NOT NULL PRIMARY KEY, context1 BIGINT, context2 BIGINT)
		
		INSERT INTO @FullContextMatch
		SELECT distinct(ctx.translation_unit_id), context1, context2
			FROM dbo.translation_unit_contexts_%% ctx
				INNER JOIN @ResultIDs res ON ctx.translation_unit_id = res.id
			WHERE context1 = @context1 AND context2 = @context2 
					and @context2 != -1 --can't have target -1
			ORDER BY translation_unit_id

		--retrive either context match for source AND target or just matches for source
		IF EXISTS(SELECT * FROM @FullContextMatch)
			SELECT translation_unit_id, context1, context2 
			FROM @FullContextMatch
		ELSE
			SELECT ctx.translation_unit_id, context1, context2 
			FROM dbo.translation_unit_contexts_%% ctx
				INNER JOIN @ResultIDs res ON ctx.translation_unit_id = res.id
			WHERE context1 = @context1 
			ORDER BY translation_unit_id
	end


	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM translation_unit_alignment_data_%% (nolock)
		WHERE translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY translation_unit_id


	DROP TABLE #CountTable

	--not required
GO

CREATE FUNCTION 
	dbo.get_feature_frequency_%%(@f int, @type INT)
RETURNS
	INT
AS
BEGIN
	DECLARE @result INT;
	
	IF @type = 1
		SELECT @result = frequency FROM dbo.ff1_%% WHERE feature = @f
	ELSE IF @type = 2
		SELECT @result = frequency FROM dbo.ff2_%% WHERE feature = @f
	ELSE IF @type = 8
		SELECT @result = frequency FROM dbo.ff8_%% WHERE feature = @f
	ELSE IF @type = 4
		SELECT @result = frequency FROM dbo.ff4_%% WHERE feature = @f

	RETURN @result
END

GO

CREATE PROCEDURE dbo.exact_search_ex_%% 
	@source_hash BIGINT, 
	@target_hash BIGINT = NULL, 
	@count INT, 
	@last_change_date DATETIME,
	@skiprows INT,
	@LeftTuContextSource BIGINT,
	@LeftTuContextTarget BIGINT,
	@idcontext nvarchar(250),
	@DescendingOrder BIT,
	@tuIdsToSkip as TuIds readonly
AS
	SET NOCOUNT ON
	
	DECLARE @ResultIDs TABLE(id INT NOT NULL PRIMARY KEY)
	
	IF @target_hash IS NOT NULL
		INSERT INTO @ResultIDs 
			SELECT  id
			FROM dbo.translation_units_%% 
			WHERE source_hash = @source_hash AND target_hash = @target_hash 
			AND
			    (@DescendingOrder=0 AND change_date >= @last_change_date or @DescendingOrder=1 AND change_date <= @last_change_date) 
			order by
				case when @DescendingOrder=0 then change_date END ASC,  
				case when @DescendingOrder=0 then id END ASC,  
				case when @DescendingOrder=1 then change_date END DESC,  
				case when @DescendingOrder=1 then id END DESC  			
			offset @skiprows ROWS
			fetch next @count rows only
			
	ELSE
	BEGIN
		IF (@idcontext = '')
			INSERT INTO @ResultIDs 
				SELECT id
				FROM dbo.translation_units_%% 
				WHERE source_hash = @source_hash AND 
					(@DescendingOrder=0 AND change_date >= @last_change_date or @DescendingOrder=1 AND change_date <= @last_change_date) 
					AND id NOT IN (select tuid from @tuIdsToSkip )  
				order by
					case when @DescendingOrder=0 then change_date END ASC,  
					case when @DescendingOrder=0 then id END ASC,  
					case when @DescendingOrder=1 then change_date END DESC,  
					case when @DescendingOrder=1 then id END DESC 
				offset @skiprows ROWS
				fetch next @count rows only
		else
			INSERT INTO @ResultIDs 
				SELECT id
				FROM dbo.translation_units_%% 
				LEFT OUTER JOIN translation_unit_idcontexts_%% as i on i.translation_unit_id = id AND idcontext = @idcontext
				WHERE source_hash = @source_hash AND   
					(@DescendingOrder=0 AND change_date >= @last_change_date or @DescendingOrder=1 AND change_date <= @last_change_date) 
				order by
					case when @DescendingOrder=0 then idcontext END ASC,  
					case when @DescendingOrder=0 then change_date END ASC,  
					case when @DescendingOrder=0 then id END ASC,  
					case when @DescendingOrder=1 then idcontext END DESC,  
					case when @DescendingOrder=1 then change_date END DESC,  
					case when @DescendingOrder=1 then id END DESC  
				offset @skiprows ROWS
				fetch next @count rows only
	END			
	
	SELECT tu.id, guid, %%, source_hash, source_segment, 
		target_hash, target_segment, creation_date, 
		creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% tu 
		INNER JOIN @ResultIDs res ON tu.id = res.id
	ORDER BY 
				case when @DescendingOrder=0 then change_date END ASC,  
				case when @DescendingOrder=0 then tu.id END ASC,  
				case when @DescendingOrder=1 then change_date END DESC,  
				case when @DescendingOrder=1 then tu.id END DESC

	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
				FROM dbo.date_attributes_%% da 
			INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 
					INNER JOIN @ResultIDs res ON da.translation_unit_id = res.id
				ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
				FROM dbo.string_attributes_%% sa 
			INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
					INNER JOIN @ResultIDs res ON sa.translation_unit_id = res.id
				ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
				FROM dbo.numeric_attributes_%% na 
			INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
					INNER JOIN @ResultIDs res ON na.translation_unit_id = res.id
				ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
			FROM dbo.picklist_attributes_%% pa 
				INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
				INNER JOIN @ResultIDs res ON pa.translation_unit_id = res.id
			ORDER BY pa.translation_unit_id, pv.attribute_id
	
	IF (@LeftTuContextSource != -1 )--can't have source -1
	BEGIN
		DECLARE @FullContextMatch TABLE(translation_unit_id INT NOT NULL PRIMARY KEY, context1 BIGINT, context2 BIGINT)
		
		INSERT INTO @FullContextMatch
		SELECT distinct(ctx.translation_unit_id), context1, context2 
			FROM dbo.translation_unit_contexts_%% ctx
				INNER JOIN @ResultIDs res ON ctx.translation_unit_id = res.id
			WHERE context1 = @LeftTuContextSource AND context2 = @LeftTuContextTarget 
					and @LeftTuContextTarget != -1 --can't have target -1
			ORDER BY translation_unit_id

		--retrive either context match for source AND target or just matches for source
		IF EXISTS(SELECT * FROM @FullContextMatch)
			SELECT translation_unit_id, context1, context2 
			FROM @FullContextMatch
		ELSE
			SELECT ctx.translation_unit_id, context1, context2 
			FROM dbo.translation_unit_contexts_%% ctx
				INNER JOIN @ResultIDs res ON ctx.translation_unit_id = res.id
			WHERE context1 = @LeftTuContextSource 
			ORDER BY translation_unit_id
	end

	IF (@idcontext != '')
		SELECT ctx.translation_unit_id, ctx.idcontext
		FROM dbo.translation_unit_idcontexts_%% ctx
			INNER JOIN @ResultIDs res ON ctx.translation_unit_id = res.id
		WHERE idcontext = @idcontext 
		ORDER BY translation_unit_id

	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM translation_unit_alignment_data_%% (nolock)
		INNER JOIN @ResultIDs res on translation_unit_id = res.id
		ORDER BY translation_unit_id
	
GO

CREATE PROCEDURE dbo.dta_search_%%
	@source_hashes  as Hashes readonly,
	@min_length TINYINT,
	@min_sigwords TINYINT,
	@count INT

AS
	SET NOCOUNT ON

	DECLARE @matchinghashes TABLE(thehash bigint NOT NULL PRIMARY KEY)
	insert into @matchinghashes SELECT distinct m.Hash FROM @source_hashes m inner join dbo.translation_unit_fragments_%% t on m.Hash = t.fragment_hash

	DECLARE @results TABLE(id INT NOT NULL, fragment_hash bigint)


	declare @thishash bigint

	SET rowcount 1

	SELECT @thishash = thehash FROM @matchinghashes

	while @@rowcount <> 0
	BEGIN
		SET rowcount 0

		insert into @results
			SELECT top(@count) t2.id, fragment_hash FROM dbo.translation_units_%% t2
				inner join dbo.translation_unit_fragments_%% f on f.translation_unit_id = t2.id
				where f.fragment_hash = @thishash
				order by change_date DESC, id DESC

		delete @matchinghashes where thehash = @thishash
		SET rowcount 1
		SELECT @thishash = thehash FROM @matchinghashes
	END
	SET rowcount 0

	
	SELECT id, fragment_hash FROM @results
		
	DECLARE @tu_ids TABLE(TuId int NOT NULL PRIMARY KEY)
	insert into @tu_ids SELECT distinct id FROM @results

	SELECT tu.id, guid, 1, source_hash, source_segment, 
		target_hash, target_segment, creation_date, 
		creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% tu  
		INNER JOIN @tu_ids res ON tu.id = res.TuId

	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
				FROM dbo.date_attributes_%% da 
			INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 
					INNER JOIN @tu_ids res ON da.translation_unit_id = res.TuId
				ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
				FROM dbo.string_attributes_%% sa 
			INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
					INNER JOIN @tu_ids res ON sa.translation_unit_id = res.TuId
				ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
				FROM dbo.numeric_attributes_%% na 
			INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
					INNER JOIN @tu_ids res ON na.translation_unit_id = res.TuId
				ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
			FROM dbo.picklist_attributes_%% pa 
				INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
				INNER JOIN @tu_ids res ON pa.translation_unit_id = res.TuId
			ORDER BY pa.translation_unit_id, pv.attribute_id
	
	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM dbo.translation_unit_alignment_data_%% (nolock)
				INNER JOIN @tu_ids res ON translation_unit_id = res.TuId
			ORDER BY translation_unit_id
	

GO


CREATE PROCEDURE dbo.get_tus_by_ids_%%
	@tu_ids as TuIds readonly

AS
	SET NOCOUNT ON
	
	
	SELECT tu.id, guid, 1, source_hash, source_segment, 
		target_hash, target_segment, creation_date, 
		creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, insert_date
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% tu (nolock)
		INNER JOIN @tu_ids res ON tu.id = res.TuId
		INNER JOIN translation_unit_alignment_data_%% align (nolock) ON tu.id = align.translation_unit_id

	
GO

CREATE PROCEDURE dbo.duplicate_search_%% @last_source_hash BIGINT, @last_tu_id INT, @count INT, @forward INT AS
	SET NOCOUNT ON
		DECLARE @ResultIDs TABLE(id INT NOT NULL PRIMARY KEY)

	IF @forward > 0
		IF @last_tu_id > 0
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
			FROM dbo.translation_units_%% 
			WHERE source_hash = @last_source_hash
				AND id > @last_tu_id ORDER BY id ASC
		ELSE
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
			FROM dbo.translation_units_%% 
			WHERE source_hash IN
				(SELECT TOP 1 source_hash FROM dbo.translation_units_%% 
					WHERE source_hash > @last_source_hash
					GROUP BY source_hash HAVING COUNT(*) > 1 ORDER BY source_hash ASC)
				ORDER BY id ASC
	ELSE
		IF @last_tu_id > 0
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
				FROM dbo.translation_units_%% WHERE source_hash = @last_source_hash
				AND id <= @last_tu_id ORDER BY id DESC
		ELSE
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
				FROM dbo.translation_units_%% WHERE source_hash IN
				(SELECT TOP 1 source_hash FROM dbo.translation_units_%% 
					WHERE source_hash <= @last_source_hash
					GROUP BY source_hash HAVING COUNT(*) > 1 ORDER BY source_hash DESC)
				ORDER BY id DESC

	SELECT dbo.translation_units_%%.id, guid, %%, source_hash, source_segment,
				target_hash, target_segment,
				creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
				, serialization_version, source_tags, target_tags, format, origin, confirmationLevel		
	FROM dbo.translation_units_%% 
		WHERE id IN (SELECT id FROM @ResultIDs) 
	
	IF  EXISTS (SELECT * FROM @ResultIDs)
	BEGIN 
	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
				FROM dbo.date_attributes_%% da 
			INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 
					INNER JOIN @ResultIDs res ON da.translation_unit_id = res.id
				ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
				FROM dbo.string_attributes_%% sa 
			INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
					INNER JOIN @ResultIDs res ON sa.translation_unit_id = res.id
				ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
				FROM dbo.numeric_attributes_%% na 
			INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
					INNER JOIN @ResultIDs res ON na.translation_unit_id = res.id
				ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
			FROM dbo.picklist_attributes_%% pa 
				INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
				INNER JOIN @ResultIDs res ON pa.translation_unit_id = res.id
			ORDER BY pa.translation_unit_id, pv.attribute_id

	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM dbo.translation_unit_alignment_data_%% (nolock)
				INNER JOIN @ResultIDs res ON translation_unit_id = res.id
			ORDER BY translation_unit_id

	--no contexts returned in duplicate search	
	END
GO

CREATE PROCEDURE dbo.duplicate_search_target_%% @last_target_hash BIGINT, @last_tu_id INT, @count INT, @forward INT AS
	SET NOCOUNT ON
		DECLARE @ResultIDs TABLE(id INT NOT NULL PRIMARY KEY)

	IF @forward > 0
		IF @last_tu_id > 0
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
			FROM dbo.translation_units_%% 
			WHERE target_hash = @last_target_hash
				AND id > @last_tu_id ORDER BY id ASC
		ELSE
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
			FROM dbo.translation_units_%% 
			WHERE target_hash IN
				(SELECT TOP 1 target_hash FROM dbo.translation_units_%% 
					WHERE target_hash > @last_target_hash
					GROUP BY target_hash HAVING COUNT(*) > 1 ORDER BY target_hash ASC)
				ORDER BY id ASC
	ELSE
		IF @last_tu_id > 0
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
				FROM dbo.translation_units_%% WHERE target_hash = @last_target_hash
				AND id <= @last_tu_id ORDER BY id DESC
		ELSE
			INSERT INTO @ResultIDs 
			SELECT TOP (@count) id
				FROM dbo.translation_units_%% WHERE target_hash IN
				(SELECT TOP 1 target_hash FROM dbo.translation_units_%% 
					WHERE target_hash <= @last_target_hash
					GROUP BY target_hash HAVING COUNT(*) > 1 ORDER BY target_hash DESC)
				ORDER BY id DESC

	SELECT dbo.translation_units_%%.id, guid, %%, source_hash, source_segment,
				target_hash, target_segment,
				creation_date, creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
				, serialization_version, source_tags, target_tags, format, origin, confirmationLevel		
	FROM dbo.translation_units_%% 
		WHERE id IN (SELECT id FROM @ResultIDs) 
	
	IF  EXISTS (SELECT * FROM @ResultIDs)
	BEGIN 
	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
				FROM dbo.date_attributes_%% da 
			INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 
					INNER JOIN @ResultIDs res ON da.translation_unit_id = res.id
				ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
				FROM dbo.string_attributes_%% sa 
			INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
					INNER JOIN @ResultIDs res ON sa.translation_unit_id = res.id
				ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
				FROM dbo.numeric_attributes_%% na 
			INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
					INNER JOIN @ResultIDs res ON na.translation_unit_id = res.id
				ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
			FROM dbo.picklist_attributes_%% pa 
				INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
				INNER JOIN @ResultIDs res ON pa.translation_unit_id = res.id
			ORDER BY pa.translation_unit_id, pv.attribute_id

	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM dbo.translation_unit_alignment_data_%% (nolock)
				INNER JOIN @ResultIDs res ON translation_unit_id = res.id
			ORDER BY translation_unit_id

	--no contexts returned in duplicate search	
	END
GO


CREATE PROCEDURE dbo.resolve_tu_guid_%% @guid UNIQUEIDENTIFIER AS
	SELECT id FROM dbo.translation_units_%% WHERE guid = @guid
GO

CREATE PROCEDURE dbo.resolve_attribute_guid_%% @guid UNIQUEIDENTIFIER AS
	SELECT id FROM dbo.attributes_%% WHERE guid = @guid
GO

CREATE PROCEDURE dbo.resolve_picklistvalue_guid_%% @guid UNIQUEIDENTIFIER AS
	SELECT id FROM dbo.picklist_values_%% WHERE guid = @guid
GO

CREATE PROCEDURE dbo.fuzzy_search_concordance_%% @features NVARCHAR(MAX), @length INT, @minScore INT, @count INT, @type INT, @last_id INT, @DescendingOrder bit AS
	DECLARE @cmd NVARCHAR(MAX)
	SET NOCOUNT ON
	DECLARE @maxLength INT, @ignored INT, @minMatches INT, @newMinMatches INT, @minHaving INT
	SET @minHaving = (SELECT value FROM dbo.parameters WHERE translation_memory_id = %% AND name = 'MINHAVING' + LTRIM(STR(@type)))
	SET @minScore = @minScore + (SELECT value FROM dbo.parameters WHERE translation_memory_id = %% AND name = 'ADDTOMINSCORE')
	IF @minScore > 100 SET @minScore = 100
	SET @minMatches = (@minScore * @length + 99) / 100
	SET @maxLength = (@length * 100 + @minScore - 1) / @minScore
	SET @newMinMatches = @minMatches
	
	DECLARE @ResultTable TABLE(id INT PRIMARY KEY, [length] int, matches INT)
	CREATE TABLE #CountTable (feature INT PRIMARY KEY, occurences INT, ignored INT DEFAULT 0)

	DECLARE @s NVARCHAR(MAX), @dataLen INT, @pos INT, @w NVARCHAR(50), @found INT
	
	SET @dataLen = (DATALENGTH(@features) / 2) - 2
	SET @s = SUBSTRING(@features, 2, @dataLen)

	SET @pos = 1
	SET @found = CHARINDEX(N',', @s, @pos)
	WHILE @found > 0
	BEGIN
		SET @w = SUBSTRING(@s, @pos, @found - @pos)
		IF  @newMinMatches > @minHaving
			INSERT INTO #CountTable VALUES(@w, dbo.get_feature_frequency_%%(@w, @type), 0)
		else
			INSERT INTO #CountTable VALUES(@w, 0, 0)
	
		SET @pos = @found + 1
		SET @found = CHARINDEX(N',', @s, @pos)
	END
	
	SET @w = SUBSTRING(@s, @pos, @dataLen + 1 - @pos)
	
	IF  @newMinMatches > @minHaving
		INSERT INTO #CountTable VALUES(@w, dbo.get_feature_frequency_%%(@w, @type), 0)
	else
		INSERT INTO #CountTable VALUES(@w, 0, 0)
		
			
	IF @length > 1
	BEGIN
		DECLARE @occ INT
		
		WHILE @newMinMatches > @minHaving
		BEGIN
			SELECT @occ = (SELECT MAX(occurences) FROM #CountTable WHERE ignored = 0)
			IF @occ IS NULL OR @occ < 1000
				BREAK;

			UPDATE TOP(1) #CountTable 
				SET ignored = 1 
				WHERE occurences = @occ
			
			SET @newMinMatches = @newMinMatches - 1
		END
	END

	SELECT @ignored = COUNT(*) FROM #CountTable WHERE ignored = 1

	IF @type = 1 -- source word index
	
	
			INSERT INTO @ResultTable 
				SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
				FROM dbo.fuzzy_index1_%% fi
					INNER JOIN #CountTable ct
					ON fi.feature = ct.feature
				WHERE 
					ct.ignored = 0
					AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
				GROUP BY fi.translation_unit_id, fi.[length]   
				HAVING COUNT(*) >= @newMinMatches
			
	ELSE IF @type = 2 -- source chars index
	
		INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index2_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches
			
	ELSE IF @type = 4 -- target words index
	
		INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index4_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches
		
	ELSE IF @type = 8 -- target chars index
	
	INSERT INTO @ResultTable 
			SELECT translation_unit_id, fi.[length], COUNT(*) AS matches 
			FROM dbo.fuzzy_index8_%% fi
				INNER JOIN #CountTable ct
				ON fi.feature = ct.feature
			WHERE 
				ct.ignored = 0
				AND (@DescendingOrder=0 AND fi.translation_unit_id > @last_id or @DescendingOrder=1 AND fi.translation_unit_id < @last_id)
			GROUP BY fi.translation_unit_id, fi.[length]   
			HAVING COUNT(*) >= @newMinMatches

	IF @ignored > 0
	BEGIN

		DECLARE @resultTopMinMatches INT
		
		SELECT @resultTopMinMatches = MIN(matches)
		FROM @ResultTable
		WHERE id IN (SELECT TOP(@count) id FROM @ResultTable ORDER BY matches DESC)

		DELETE FROM @ResultTable
		WHERE 
			matches + @ignored < @minMatches
			OR matches + @ignored < @resultTopMinMatches
				
		IF @type = 1
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index1_%% fi WITH (INDEX = fi1_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
				WHERE c.ignored = 1)
		ELSE IF @type = 2
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index2_%% fi WITH (INDEX = fi2_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
                WHERE c.ignored = 1)
		ELSE IF @type = 8
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index8_%% fi WITH (INDEX = fi8_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
                WHERE c.ignored = 1)
		ELSE IF @type = 4
			UPDATE @ResultTable SET matches = matches + (SELECT COUNT(*) FROM dbo.fuzzy_index4_%% fi WITH (INDEX = fi4_address_%%)
				JOIN @ResultTable r ON r.id = fi.translation_unit_id 
                JOIN #CountTable c ON fi.feature = c.feature 
                WHERE c.ignored = 1)
    END
	
	DECLARE @ResultIDs TABLE(id INT)
	IF @DescendingOrder = 1
	BEGIN
		INSERT INTO @ResultIDs 
		SELECT TOP (@count) rt.id 
		FROM @ResultTable rt
		WHERE rt.matches >= @minMatches 
		ORDER BY rt.matches DESC, rt.id desc
	END
	else
	BEGIN
		INSERT INTO @ResultIDs 
		SELECT TOP (@count) rt.id 
		FROM @ResultTable rt
		WHERE rt.matches >= @minMatches 
		ORDER BY rt.matches DESC, rt.id ASC
	END

	SELECT id, guid, 1, source_hash, source_segment,
		target_hash, target_segment,
		creation_date, creation_user, change_date, change_user, 
		last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% 
			WHERE id IN (SELECT id FROM @ResultIDs) 
	
	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value
		FROM dbo.date_attributes_%% da INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id
		WHERE da.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value
		FROM dbo.string_attributes_%% sa INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
		WHERE sa.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value
		FROM dbo.numeric_attributes_%% na INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
		WHERE na.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
		FROM dbo.picklist_attributes_%% pa 
			INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id	
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id	
		WHERE pa.translation_unit_id IN (SELECT id FROM @ResultIDs) 
		ORDER BY pa.translation_unit_id, pv.attribute_id


	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM dbo.translation_unit_alignment_data_%% (nolock)
				INNER JOIN @ResultIDs res ON translation_unit_id = res.id
			ORDER BY translation_unit_id

	DROP TABLE #CountTable

	--TUContext not required for fuzzy_concordance search
GO

CREATE PROCEDURE dbo.get_tus_ToAlign_%% 
	@start_after INT, 
	@count INT,
	@unalignedorpostdated BIT,
	@unaligned BIT
AS
	SET NOCOUNT ON
	
	DECLARE @ResultIDs TABLE(id INT NOT NULL PRIMARY KEY)	

	IF @unaligned = 1 BEGIN  
		INSERT INTO @ResultIDs 
			SELECT TOP (@count) translation_unit_id
			FROM dbo.translation_unit_alignment_data_%% (nolock)
			WHERE translation_unit_id > @start_after AND 
				(alignment_data is NULL)
			ORDER BY translation_unit_id ASC	
		END
	ELSE IF @unalignedorpostdated = 1 BEGIN  
		INSERT INTO @ResultIDs 
			SELECT TOP (@count) translation_unit_id
			FROM dbo.translation_unit_alignment_data_%% (nolock)
			WHERE translation_unit_id > @start_after AND 
				(alignment_data is NULL or (align_model_date < insert_date))
			ORDER BY translation_unit_id ASC	
	END
	ELSE BEGIN  
		INSERT INTO @ResultIDs 
			SELECT TOP (@count) translation_unit_id
			FROM dbo.translation_unit_alignment_data_%% (nolock)
			WHERE translation_unit_id > @start_after 
			ORDER BY translation_unit_id ASC	
	END

	SELECT id, guid, source_hash, source_segment, target_hash, target_segment, 	
		 source_token_data, target_token_data, null, null, insert_date
		  , serialization_version, source_tags, target_tags
	FROM dbo.translation_units_%% INNER JOIN dbo.translation_unit_alignment_data_%% (nolock) ON translation_unit_id = id
	WHERE id IN (SELECT id FROM @ResultIDs)	
		
GO

CREATE PROCEDURE dbo.sp_GetAlignmentTimestampsByIds_%%
@tu_ids AS TuIds READONLY
AS
	SELECT tu.id, alignment.insert_date
	FROM dbo.translation_units_%% tu 
		INNER JOIN @tu_ids as params
			on tu.id = params.TuId
		INNER JOIN dbo.translation_unit_alignment_data_%% (nolock) alignment 
			ON tu.id = alignment.translation_unit_id		
	WHERE alignment.alignment_data IS NULL	
GO

CREATE PROCEDURE dbo.sp_GetAlignmentTimestampsPaginated_%% 
	@start_after INT, 
	@count INT,
	@model_date DATETIME
AS
	SET NOCOUNT ON
	SET DEADLOCK_PRIORITY LOW;  
	
	SELECT TOP (@count) translation_unit_id, insert_date
	FROM dbo.translation_unit_alignment_data_%% (nolock)	
	WHERE 		
		 translation_unit_id > @start_after AND -- pagination
		(alignment_data is null OR (align_model_date < insert_date and insert_date < @model_date))
	ORDER BY translation_unit_id		

	SET DEADLOCK_PRIORITY NORMAL;  
GO


CREATE PROCEDURE dbo.batch_exact_search_%%
	@source_hashes as Hashes readonly,
	@filterParam as TUFilterParams readonly

AS
	SET NOCOUNT ON
	
	 declare @FilteredIDs as TuIDs  

	declare @filterParamCount int, @applyHardFilter bit
	select @filterParamCount = count(*) from @filterParam
	
	SET @applyHardFilter = CASE 
                 WHEN ( @filterParamCount > 0 ) THEN 1
                 ELSE 0
              END
	if (@applyHardFilter = 1)
		--first 4 parameters are about pagination that is not used in this call
		insert into @FilteredIDs
		exec dbo.getFilteredTus_%% 0, 0, 0, 0, @filterParam

	SELECT id, source_hash, source_segment, target_segment, source_token_data, target_token_data
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% AS tu
		INNER JOIN @source_hashes AS hashes
			ON tu.source_hash = hashes.Hash
		LEFT JOIN @FilteredIDs as filteredTu
			on @applyHardFilter = 1 and filteredTu.TuId = tu.id
		WHERE (@applyHardFilter = 0 and filteredTu.TuId is null) or 
			  (@applyHardFilter = 1 and filteredTu.TuId is not null)
	 
GO

CREATE PROCEDURE dbo.batch_duplicate_search_%%
	@tuid_hashes as TuIDHashes readonly

AS
	SET NOCOUNT ON
	

	SELECT tu.id, source_hash, target_hash, source_segment, target_segment, source_token_data, target_token_data
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
	FROM dbo.translation_units_%% AS tu
		INNER JOIN @tuid_hashes AS hashes
			ON tu.source_hash = hashes.sourceHash
			AND tu.target_hash = hashes.targetHash
	 
GO

CREATE PROCEDURE dbo.get_full_tus_by_ids_%% 
	@tu_ids as TuIds readonly

AS
	SET NOCOUNT ON


	DECLARE @ResultIDs TABLE(id INT NOT NULL PRIMARY KEY)
	
		INSERT INTO @ResultIDs 
			SELECT tu.id
			FROM dbo.translation_units_%% tu
			INNER JOIN @tu_ids res ON tu.id = res.TuId
	
		SELECT tu.id, guid, %%, source_hash, source_segment, 
			target_hash, target_segment, creation_date, 
			creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
			, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
		FROM dbo.translation_units_%% tu 
			INNER JOIN @ResultIDs res ON tu.id = res.id

	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
				FROM dbo.date_attributes_%% da 
			INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 
					INNER JOIN @ResultIDs res ON da.translation_unit_id = res.id
				ORDER BY da.translation_unit_id, da.attribute_id
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
				FROM dbo.string_attributes_%% sa 
			INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
					INNER JOIN @ResultIDs res ON sa.translation_unit_id = res.id
				ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
				FROM dbo.numeric_attributes_%% na 
			INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
					INNER JOIN @ResultIDs res ON na.translation_unit_id = res.id
				ORDER BY na.translation_unit_id, na.attribute_id
		
	SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id 
			FROM dbo.picklist_attributes_%% pa 
				INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id
			INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
				INNER JOIN @ResultIDs res ON pa.translation_unit_id = res.id
			ORDER BY pa.translation_unit_id, pv.attribute_id

	SELECT distinct(ctx.translation_unit_id), context1, context2
			FROM dbo.translation_unit_contexts_%% ctx
				INNER JOIN @ResultIDs res ON ctx.translation_unit_id = res.id
				ORDER BY ctx.translation_unit_id

	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM translation_unit_alignment_data_%% (nolock)
		INNER JOIN @ResultIDs res on translation_unit_id = res.id
		ORDER BY translation_unit_id


	 
GO

CREATE PROCEDURE dbo.get_tus_WithHashes_%% 
	@TUID_hashes AS TuIDHashes readonly,
	@returnIdContext INT
AS
	SET NOCOUNT ON

	DECLARE @TUID_unique TABLE(id INT)
	INSERT INTO @TUID_unique(id)
	SELECT DISTINCT id
	FROM @TUID_hashes



	--TU data
	SELECT TU.id, guid, %%, source_hash, source_segment, 0, 0, 
		target_hash, target_segment, 0, 0, creation_date, 
		creation_user, change_date, change_user, last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags, format, origin, confirmationLevel
		FROM dbo.translation_units_%% as TU
			INNER JOIN @TUID_unique as TUIds ON TU.id = TUIds.id

	--TUContext
		-- retrieve context data for tus that have at least source-context match
			DECLARE @SourceContextMatch TABLE(translation_unit_id INT NOT NULL, context1 BIGINT, context2 BIGINT)

			INSERT INTO @sourceContextMatch
			SELECT ctx.translation_unit_id, ctx.context1, ctx.context2 
			FROM dbo.translation_unit_contexts_%% ctx
				INNER JOIN @TUID_hashes AS TUIds 
						on TUIds.sourceHash != -1 and
						    TUIds.id = ctx.translation_unit_id AND							
							TUIds.sourceHash = ctx.context1


			SELECT translation_unit_id, context1, context2
			FROM @sourceContextMatch

	IF @returnIdContext = 1		
		SELECT translation_unit_id, idcontext
			FROM dbo.translation_unit_idcontexts_%% 
			WHERE	translation_unit_id IN (SELECT id FROM @TUID_unique) 
			ORDER BY translation_unit_id

	--TU attributes
		   SELECT  da.translation_unit_id, da.attribute_id, a.name, a.type, da.value
			  FROM date_attributes_%% da 
						INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id
						INNER JOIN @TUID_unique AS TUIds ON da.translation_unit_id = TUIds.id
			ORDER BY da.translation_unit_id, da.attribute_id

		   SELECT sa.translation_unit_id, sa.attribute_id,a.name, a.type, sa.value
			  FROM string_attributes_%% sa 
						INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id
						INNER JOIN @TUID_unique AS TUIds ON sa.translation_unit_id = TUIds.id
			ORDER BY sa.translation_unit_id, sa.attribute_id

		  SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value
			  FROM numeric_attributes_%% na 
						INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id
						INNER JOIN @TUID_unique AS TUIds ON na.translation_unit_id = TUIds.id
		  ORDER BY na.translation_unit_id, na.attribute_id

		  SELECT pa.translation_unit_id, pv.attribute_id, a.name, a.type, pa.picklist_value_id
			  FROM picklist_attributes_%% pa 
					INNER JOIN picklist_values_%% pv ON pv.id = pa.picklist_value_id
					INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
					INNER JOIN @TUID_unique AS TUIds ON pa.translation_unit_id = TUIds.id
			ORDER BY pa.translation_unit_id, pv.attribute_id


	

		-- alignment data not needed - used for 'apply tm' only
		 
GO

CREATE PROCEDURE dbo.clear_alignment_data_%% 
AS
	UPDATE	translation_unit_alignment_data_%% SET alignment_data = null , align_model_date = NULL
	DELETE FROM translation_unit_fragments_%%

GO

CREATE PROCEDURE dbo.get_postdated_tu_count_%%
	@dateVal as DATETIME
AS
	SET NOCOUNT ON
	select count (*) from translation_unit_alignment_data_%% (nolock) where @dateVal < insert_date

GO

CREATE PROCEDURE dbo.get_aligned_predated_tu_count_%%
	@model_date as DATETIME
AS
	SET NOCOUNT ON
	-- finds aligned tus that were aligned by a model built before they were inserted (i.e. they were 'postdated')
	-- but which were inserted before the model was rebuilt (i.e. they are now 'predated')
	-- ==> their OOV tokens should be covered by the new model and so they are worth aligning again
	select count (*) from translation_unit_alignment_data_%% (nolock)
	where insert_date < @model_date AND align_model_date < insert_date

GO

CREATE PROCEDURE dbo.sp_get_unaligned_tu_count_%%
	@model_date DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON
	
	IF @model_date IS NOT NULL
	BEGIN 
		SELECT SUM(QTY)
		FROM (
			SELECT COUNT (*) qty 
			FROM translation_unit_alignment_data_%% (nolock)
			WHERE 1=1
				AND (alignment_data is null )		
			UNION ALL
			SELECT COUNT (*) qty 
			FROM translation_unit_alignment_data_%% (nolock)
			WHERE 1=1
				AND align_model_date < insert_date 
				AND insert_date < @model_date		
			) Q
	END
	ELSE 
	BEGIN
		SELECT COUNT (*) qty 
		FROM translation_unit_alignment_data_%% (nolock)
		WHERE 1=1
			AND (alignment_data is null )	
	END
	;
END
GO

CREATE PROCEDURE [dbo].[get_tus_ex_f_attrsql_%%]
	@tablePrefix varchar(10),
	@tableSuffix varchar(10),
	@typePrefix varchar(200),
	@attributes as TUFilterParams readonly,
	@attributeType varchar(50),
	@withs nvarchar(max) out,
	@columns nvarchar(max) out,
	@from nvarchar(max) out
AS
BEGIN
    SET NOCOUNT ON

	SELECT @withs = @withs + '[' + @tablePrefix + 'a_' + [value] + @tableSuffix + '] as 
		(
			SELECT tu.id, ' + @tablePrefix + 'a.value [' + [value] + ']
			FROM translation_units_%% tu  
			left join ' + @typePrefix + '_attributes_%% ' + @tablePrefix + 'a on ' + @tablePrefix + 'a.translation_unit_id = tu.id  
			where ' + @tablePrefix + 'a.attribute_id = (SELECT id FROM attributes_%% where [name] = ''' + [value] + ''')	
		),'
	FROM @attributes where [type] = @attributeType

	SELECT @from = @from + ' left join [' + @tablePrefix + 'a_' + [value] + '] on [' + @tablePrefix + 'a_' + [value] + '].id = tu.id ',
		   @columns = @columns + ', [' + [value] + ']'
	FROM @attributes where [type] = @attributeType	
END
GO

CREATE PROCEDURE dbo.getFilteredTus_%%
	@pagination BIT,
    @start_after INT, 
	@count INT, 
	@forward INT,	
	@filterParam as TUFilterParams readonly
as
begin

	declare @systemFields nvarchar(max), @dateAttributes nvarchar(max), @numericAttributes nvarchar(max), @singlePickListAttributes nvarchar(max)
	declare @multiPickListAttributes nvarchar(max), @stringAttributes nvarchar(max), @multiStringAttributes nvarchar(max)
	declare @filterExpression nvarchar(max), @havings nvarchar(max)

	select @systemFields = COALESCE(@systemFields + ', ', '') + [value]  from @filterParam where [type] = 'systemFields'
	select @dateAttributes = COALESCE(@dateAttributes + ', ', '') + [value]  from @filterParam where [type] = 'dateAttributes'
	select @numericAttributes = COALESCE(@numericAttributes + ', ', '') + [value]  from @filterParam where [type] = 'numericAttributes'
	select @singlePickListAttributes = COALESCE(@singlePickListAttributes + ', ', '') + [value]  from @filterParam where [type] = 'singlePickListAttributes'
	select @multiPickListAttributes = COALESCE(@multiPickListAttributes + ', ', '') + [value]  from @filterParam where [type] = 'multiPickListAttributes'
	select @stringAttributes = COALESCE(@stringAttributes + ', ', '') + [value]  from @filterParam where [type] = 'stringAttributes'
	select @multiStringAttributes = COALESCE(@multiStringAttributes + ', ', '') + [value]  from @filterParam where [type] = 'multiStringAttributes'
	select @filterExpression = [value] from @filterParam where [type] = 'filterExpression'
	select @havings = [value] from @filterParam where [type] = 'havings'	

	declare @select nvarchar(100) ='',
			@from nvarchar(max) = '',
			@wheres nvarchar(max) = '',
		    @columns nvarchar(max) = '',
			@withs nvarchar(max) = '',
			@filteredQuery nvarchar(max) = ''
	SET @from = 'FROM translation_units_%% tu '

	if (@pagination = 1)
	begin
		set @select = 'SELECT top (@count) tu.id '
		IF @forward > 0
			SET @wheres = ' WHERE tu.id > @start_after'
		ELSE
			SET @wheres = ' WHERE tu.id <= @start_after'
	END
	ELSE
		set @select = 'SELECT tu.id '

	IF (len(@filterExpression) > 0)
	BEGIN
		if (@pagination = 1) 
			SELECT @wheres = @wheres + ' AND (' + @filterExpression + ')'
		else
			SELECT @wheres = ' where ' + @filterExpression
	END


	IF (len(@dateAttributes) > 0)
	BEGIN
		exec dbo.[get_tus_ex_f_attrsql_%%] 'd', '', 'date', @filterParam, 'dateAttributes', @withs out, @columns out, @from out
	END

	IF (len(@numericAttributes) > 0)
	BEGIN
		exec dbo.[get_tus_ex_f_attrsql_%%] 'n', '', 'numeric', @filterParam, 'numericAttributes', @withs out, @columns out, @from out
	END

	IF (len(@stringAttributes) > 0)
	BEGIN
		exec dbo.[get_tus_ex_f_attrsql_%%] 's', '', 'string', @filterParam, 'stringAttributes', @withs out, @columns out, @from out
	END	

	IF (len(@multiStringAttributes) > 0)
	BEGIN
		exec dbo.[get_tus_ex_f_attrsql_%%] 'ms', '_tmp', 'string', @filterParam, 'multiStringAttributes', @withs out, @columns out, @from out
		
		SELECT @withs = @withs + '[msa_' + [value] + '] as 
		(
			SELECT t.id, t.[' + [value] + '], c.tc [' + [value] + '_total]
			FROM [msa_' + [value] + '_tmp] t
			inner join (SELECT id, count(*) tc FROM [msa_' + [value] + '_tmp] group by id) c
			on t.id = c.id
		),',
			@columns = @columns + ',[' + [value] + '_total], count(distinct [' + [value] + ']) [' + [value] + '_match] '			   
		FROM @filterParam where [type] = 'multiStringAttributes'
	END	

	IF (len(@singlePickListAttributes) > 0)
	BEGIN		
		SELECT @withs = @withs + '[spa_' + [value] + '] as 
		(
			SELECT tu.id, pv.value [' + [value] + ']
			FROM translation_units_%% tu  
			left join picklist_attributes_%% pa on pa.translation_unit_id = tu.id  
			left join picklist_values_%% pv on pa.picklist_value_id = pv.id
			where pv.attribute_id = (SELECT [id] FROM attributes_%% where [name] = ''' + [value] + ''')	
		),'
		FROM @filterParam where [type] = 'singlePickListAttributes'

		SELECT @from = @from + ' left join [spa_' + [value] + '] on [spa_' + [value] + '].id = tu.id ',
			   @columns = @columns + ', [' + [value] + ']'
		FROM @filterParam where [type] = 'singlePickListAttributes'
	END	

	IF (len(@multiPickListAttributes) > 0)
	BEGIN
		SELECT @withs = @withs + '[mpa_' + [value] + '_tmp] as 
		(
			SELECT tu.id, pv.value [' + [value] + ']
			FROM translation_units_%% tu  
			left join picklist_attributes_%% pa on pa.translation_unit_id = tu.id  
			left join picklist_values_%% pv on pa.picklist_value_id = pv.id
			where pv.attribute_id = (SELECT [id] FROM attributes_%% where [name] = ''' + [value] + ''')	
		),		
		[mpa_' + [value] + '] as 
		(
			SELECT t.id, t.[' + [value] + '], c.tc [' + [value] + '_total]
			FROM [mpa_' + [value] + '_tmp] t
			inner join (SELECT id, count(*) tc FROM [mpa_' + [value] + '_tmp] group by id) c
			on t.id = c.id
		),'
		FROM @filterParam where [type] = 'multiPickListAttributes'

		SELECT @from = @from + ' left join [mpa_' + [value] + '] on [mpa_' + [value] + '].id = tu.id ',
			   @columns = @columns + ', [' + [value] + ']'
		FROM @filterParam where [type] = 'multiPickListAttributes'
	END	
	
	

	IF (len(@systemFields) > 0)
	BEGIN
		SELECT @columns = @columns + ', ' + [value]
		FROM @filterParam where [type] = 'systemFields'
	END	

	IF (len(@withs) > 0)
	BEGIN
		SET @filteredQuery = ';with ' + left(@withs, len(@withs) - 1)
	END
	
	SET @filteredQuery = @filteredQuery 
		+ @select
		+ @from + @wheres +
		+ ' group by tu.id ' + case when len(@havings) > 0 then 'having' + @havings else '' END
		+ case when @forward > 0 then ' order by tu.id asc ' else 'order by tu.id desc' END     

	exec sp_executesql @filteredQuery,
					   N'@count int, @start_after int, @dateAttributes varchar(max), @numericAttributes varchar(max), @singlePickListAttributes varchar(max), @multiPickListAttributes varchar(max), @stringAttributes varchar(max), @multiStringAttributes varchar(max)',
					   @count, @start_after, @dateAttributes, @numericAttributes, @singlePickListAttributes, @multiPickListAttributes, @stringAttributes, @multiStringAttributes			
END
GO

CREATE PROCEDURE [dbo].[get_tus_ex_f_%%] 
	@start_after INT, 
	@count INT, 
	@forward INT,
	@filterParam as TUFilterParams readonly,	
	@returnIdContext INT,
	@includeContextContent INT,
	@cm_is_preceding_following BIT
AS
BEGIN
	create table #ResultIDs (TuId INT NOT NULL PRIMARY KEY) 

	insert into #ResultIDs
	exec dbo.getFilteredTus_%% 1, @start_after, @count, @forward, @filterParam

	SELECT id, guid, %%, source_hash, source_segment, 
		target_hash, target_segment, 
		creation_date, creation_user, change_date, change_user, 
		last_used_date, last_used_user, usage_counter, source_token_data, target_token_data, null, null, null
		, serialization_version, source_tags, target_tags,  format, origin, confirmationLevel
	FROM dbo.translation_units_%% 
	WHERE id IN (SELECT TuId FROM #ResultIDs)
	
	SELECT da.translation_unit_id, da.attribute_id, a.name, a.type, da.value 
		FROM dbo.date_attributes_%% da INNER JOIN dbo.attributes_%% a ON da.attribute_id = a.id 	
		WHERE da.translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
		ORDER BY da.translation_unit_id, da.attribute_id		
		
	SELECT sa.translation_unit_id, sa.attribute_id, a.name, a.type, sa.value 
		FROM dbo.string_attributes_%% sa INNER JOIN dbo.attributes_%% a ON sa.attribute_id = a.id 		
		WHERE sa.translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
		ORDER BY sa.translation_unit_id, sa.attribute_id
		
	SELECT na.translation_unit_id, na.attribute_id, a.name, a.type, na.value 
	FROM dbo.numeric_attributes_%% na  INNER JOIN dbo.attributes_%% a ON na.attribute_id = a.id 
		WHERE na.translation_unit_id IN (SELECT TuId FROM #ResultIDs) 

	ORDER BY na.translation_unit_id, na.attribute_id
	
	SELECT pa.translation_unit_id, pv.attribute_id,  a.name, a.type, pa.picklist_value_id 
	FROM dbo.picklist_attributes_%% pa 
		INNER JOIN dbo.picklist_values_%% pv ON pv.id = pa.picklist_value_id		
		INNER JOIN dbo.attributes_%% a ON pv.attribute_id = a.id
		WHERE pa.translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
	ORDER BY pa.translation_unit_id, pv.attribute_id

	IF @includeContextContent = 1
	BEGIN
		IF @cm_is_preceding_following = 1
		BEGIN
			SELECT translation_unit_id, context1, context2, t1.source_segment, t1.source_tags, t1.source_token_data, t2.source_segment, t2.source_tags, t2.source_token_data, t1.serialization_version, t2.serialization_version
				FROM dbo.translation_unit_contexts_%% 
				LEFT OUTER JOIN translation_units_%% As t1
				ON t1.id = (select max(id) from translation_units_%% where source_hash = context1)
				LEFT OUTER JOIN translation_units_%% As t2
				ON t2.id = (select max(id) from translation_units_%% where source_hash = context2)
				WHERE	translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
				ORDER BY translation_unit_id
		END
		ELSE
		BEGIN
			SELECT translation_unit_id, context1, context2, source_segment, source_tags, source_token_data, target_segment, target_tags, target_token_data, serialization_version
				FROM dbo.translation_unit_contexts_%% 
				LEFT OUTER JOIN translation_units_%%
				ON id = (select max(id) from translation_units_%% where source_hash = context1 and target_hash = context2)
				WHERE	translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
				ORDER BY translation_unit_id
		END
	END
	ELSE
	BEGIN
		SELECT translation_unit_id, context1, context2 
			FROM dbo.translation_unit_contexts_%% 
			WHERE	translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
			ORDER BY translation_unit_id
	END

	IF @returnIdContext = 1		
		SELECT translation_unit_id, idcontext
			FROM dbo.translation_unit_idcontexts_%% 
			WHERE	translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
			ORDER BY translation_unit_id

	SELECT translation_unit_id, alignment_data, align_model_date, insert_date
		FROM dbo.translation_unit_alignment_data_%% (nolock)
		WHERE	translation_unit_id IN (SELECT TuId FROM #ResultIDs) 
		ORDER BY translation_unit_id

END
	
GO

CREATE PROCEDURE dbo.batch_fuzzy_search_%%
		@BatchFeatures as TuIDFeatures readonly, @minScore INT,  @pageSize int,
		@filterParam as TUFilterParams readonly
AS	
	SET NOCOUNT ON
	DECLARE  @startID int, @finishID int -- batch variables
	DECLARE  @searchTUId int, @searchFeatureCount int, @maxLengthPerTU INT, @minMatchesPerTU INT -- TU variables

	DECLARE @singleTUFeatures as TuIDFeatures -- TUID, feature	
	declare @FilteredIDs as TuIDs  -- these are filtered TUs that apply to all TUs inside the batch
	DECLARE @ResultIDs TABLE(SearchId INT, TuID INT) -- search TUID, Matching TM TUID
	DECLARE @BatchIterator		table(id int primary key Identity(1,1), TUId INT) -- iteratorkey, TUID	
	
	
	declare @filterParamCount int, @applyHardFilter bit
	select @filterParamCount = count(*) from @filterParam
	
	SET @applyHardFilter = CASE 
                 WHEN ( @filterParamCount > 0 ) THEN 1
                 ELSE 0
              END
	
	if (@applyHardFilter = 1)
		begin		
			insert into @FilteredIDs
			--first 4 parameters are about pagination that is not used in this call
			exec dbo.getFilteredTus_%% 0, 0, 0, 0, @filterParam	
		end
		
	SET @minScore = @minScore + (SELECT value FROM dbo.parameters WHERE translation_memory_id = %% AND name = 'ADDTOMINSCORE')
	IF @minScore > 100 SET @minScore = 100	
		
	--prepare batch iterator
	insert into @BatchIterator(TUId)
		select distinct(TuId) from @BatchFeatures
	select @startID = min(id), @finishID = max(id) from @BatchIterator

	while @startID <= @finishID 
	begin
		select @searchTUId = TuId from @BatchIterator where id = @startID

		insert into @singleTUFeatures
		select * from @BatchFeatures where TuID = @searchTUId

		select @searchFeatureCount = count(*) from @singleTUFeatures
		SET @minMatchesPerTU = (@minScore * @searchFeatureCount + 99) / 100
		SET @maxLengthPerTU = (@searchFeatureCount * 100 + @minScore - 1) / @minScore		

		--apply the filters 		
		
		insert into @ResultIDs -- get results for current batch
		select @searchTUId, TuId 
		from singleTU_fuzzy_search_%%(@singleTUFeatures, @minScore, @minMatchesPerTU, @maxLengthPerTU, @pageSize, @applyHardFilter, @FilteredIDs)

		set @startID = @startID + 1

		delete from @singleTUFeatures
	End

	select id, source_hash, source_segment, target_segment, source_token_data, target_token_data, 
		serialization_version, source_tags, target_tags, format, origin, confirmationLevel, SearchId -- retrieve 13 columns for Matching TU(reduced form), the last column stores the searching TU
	from @ResultIDs
		inner join translation_units_%% on
			TuID = id
GO

CREATE function dbo.singleTU_fuzzy_search_%% (@TuIDFeatures as TuIDFeatures readonly, @minScore INT, @minMatchesPerTU int,  @maxLengthPerTU int, @pageSize int, @applyHardFilter bit, @FilteredIDs  as TuIds readonly)
RETURNS @results TABLE   
(  
    TUId int NOT NULL    
)   
AS
begin
	DECLARE @ignored INT, @MinMatches INT, @adjustedMinMatches int, @minHaving INT
		
	DECLARE @TUMatches TABLE(id INT PRIMARY KEY, [length] int, matchCount INT)
	declare @SearchFeatures Table(feature INT PRIMARY KEY, occurences INT, ignored INT DEFAULT 0)	

	SET @minHaving = (SELECT [value] FROM dbo.parameters WHERE translation_memory_id = %% AND [name] = 'MINHAVING1')	

	set @MinMatches = @minMatchesPerTU
	set @adjustedMinMatches = @minMatchesPerTU	
	----------------------------------------------------------------------------------------
	------load search data in @SearchFeatures 
	if (@adjustedMinMatches > @minHaving) -- for score 50, segment length minimum 7
		  insert into @SearchFeatures     
		  select SearchFeature.Feature, TMFeatureCounter.frequency, 0  
		  from @TuIDFeatures SearchFeature left join ff1_%% TMFeatureCounter  
			on SearchFeature.Feature = TMFeatureCounter.Feature  
	else				
		insert into @SearchFeatures 
		select Feature, 0, 0
		from @TuIDFeatures		
	
	----------------------------------------------------------------------------------------		
	IF @maxLengthPerTU > 1
	BEGIN
		DECLARE @noiseCandidateCounter INT
		
		WHILE @adjustedMinMatches > @minHaving
		BEGIN
			SELECT @noiseCandidateCounter = (SELECT MAX(occurences) FROM @SearchFeatures WHERE ignored = 0)
			IF @noiseCandidateCounter IS NULL OR @noiseCandidateCounter < 1000 -- noise threadshold
				BREAK; -- not a valid candidate 

			UPDATE TOP(1) @SearchFeatures -- candidate confirmed, remove the feature from the search
			SET ignored = 1 
			WHERE occurences = @noiseCandidateCounter
			
			SET @adjustedMinMatches = @adjustedMinMatches - 1 -- reduce the score requirement since we dropped a search feature
		END
	END

	SELECT @ignored = COUNT(*) FROM @SearchFeatures WHERE ignored = 1 -- counter for ignored search features 
	
	--------do the matching but skip the ignore features
	INSERT INTO @TUMatches 
	SELECT translation_unit_id, tmFI.[length], COUNT(*) AS matchCount
	FROM dbo.fuzzy_index1_%% tmFI
		INNER JOIN @SearchFeatures searchFI
			ON tmFI.feature = searchFI.feature
		LEFT JOIN @FilteredIDs filteredTU -- join runs only if @applyHardFilter = 1
			ON @applyHardFilter = 1 and filteredTU.TuId = tmFI.translation_unit_id
	WHERE 
		searchFI.ignored = 0
		AND ((@applyHardFilter = 0 and filteredTU.TuId is null) or (@applyHardFilter = 1 and filteredTU.TuId is not null))
		AND tmFI.length BETWEEN @minMatches AND @maxLengthPerTU 				
	GROUP BY tmFI.translation_unit_id, tmFI.[length]   
	HAVING COUNT(*) >= @adjustedMinMatches -- apply the score adjusted by noise (for medium/long segments), notice the search area is expanded
		
	IF @ignored > 0 -- for score 50, segment length minimum 7, segment has noise words that means 1000 occurences in TM
	BEGIN

		DECLARE @resultTopMinMatches INT
		
		SELECT @resultTopMinMatches = MIN(matchCount) --get smallest match count from the first best page
		FROM @TUMatches
		WHERE id IN (SELECT TOP(@pageSize) id FROM @TUMatches ORDER BY matchCount DESC)

		DELETE FROM @TUMatches -- remove all matches that don't have a chance even if the dropped features(@ignored count) are considered
		WHERE 
			matchCount + @ignored < @minMatches
			OR matchCount + @ignored < @resultTopMinMatches				

		UPDATE @TUMatches -- do the matching for the ignored features and adjust the TU match 
		SET matchCount = matchCount + (SELECT COUNT(*) 
								FROM dbo.fuzzy_index1_%% tmFI WITH (INDEX = fi1_address_%%)
									JOIN @TUMatches matches ON matches.id = tmFI.translation_unit_id 
									JOIN @SearchFeatures searchFI ON tmFI.feature = searchFI.feature 
									LEFT JOIN @FilteredIDs filteredTU -- join runs only if @applyHardFilter = 1
										ON @applyHardFilter = 1 and filteredTU.TuId = tmFI.translation_unit_id
									WHERE searchFI.ignored = 1
										AND ((@applyHardFilter = 0 and filteredTU.TuId is null) or (@applyHardFilter = 1 and filteredTU.TuId is not null))
								)
		
    END

	INSERT INTO @results 
	SELECT TOP (@pageSize) id 
	FROM @TUMatches 
	WHERE matchCount >= @minMatches 
	order by matchCount desc

	return
	end
GO

create view vtranslation_units_flags_%%
AS
SELECT id, guid,
	case origin 
				when 0 then 'Unknown'
				when 1 then 'TM'
				when 2 then 'MachineTranslation'
				when 3 then 'Alignment'
				when 4 then 'ContextTM'
				when 5 then 'AdaptiveMachineTranslation'
				when 6 then 'Nmt'
				when 7 then 'AutomaticTranslation'
				else 'error' + cast(origin as char(1))
	 end AS 'origin',
	 case format 
				when 0 then 'SDLTradosStudio2009'
				when 1 then 'Unknown'
				when 2 then 'SDLX'
				when 3 then 'TradosTranslatorsWorkbench'
				when 4 then 'IdiomWorldServer'
				when 5 then 'TradosTTX'
				when 6 then 'SDLItd'
				else 'error' + cast(format as char(1))
	 end AS 'format',
	 case confirmationLevel 
				when 0 then 'Unspecified'
				when 1 then 'Draft'
				when 2 then 'Translated'
				when 3 then 'RejectedTranslation'
				when 4 then 'ApprovedTranslation'
				when 5 then 'RejectedSignOff'
				when 6 then 'ApprovedSignOff'
				else 'error' + cast(confirmationLevel as char(1))
	 end AS 'confirmationLevel' 
FROM translation_units_%%
GO

CREATE PROCEDURE dbo.sp_addUpdate_tu_lastSearch_%%
@tu_ids as TuIds READONLY,
@lastSearch AS DATETIME
AS
	 SET NOCOUNT ON
	 SELECT Ids.tuId, CASE WHEN tudata.translation_unit_id IS NULL 
					  THEN 0	ELSE 1 END AS 'update'
	 INTO #MatchingIds
	 FROM @tu_ids Ids 
		LEFT JOIN translation_unit_last_search_%% AS tudata
			ON Ids.tuId = tudata.translation_unit_id

	INSERT INTO translation_unit_last_search_%%(translation_unit_id, last_search_on)
	SELECT tuId, @lastSearch
	FROM #MatchingIds 
	WHERE [update] = 0

	UPDATE translation_unit_last_search_%%
	SET last_search_on = @lastSearch
	FROM translation_unit_last_search_%% 
		INNER JOIN #MatchingIds 
			ON [update] = 1	AND tuId = translation_unit_id


	DROP TABLE #MatchingIds	
	SET NOCOUNT OFF
GO