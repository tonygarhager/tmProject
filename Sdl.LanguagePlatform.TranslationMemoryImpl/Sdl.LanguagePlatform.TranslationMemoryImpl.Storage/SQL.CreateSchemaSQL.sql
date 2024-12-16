----- Resources
CREATE TABLE dbo.resources(
	id INT IDENTITY NOT NULL PRIMARY KEY,
	guid UNIQUEIDENTIFIER NOT NULL UNIQUE,
	type INT NOT NULL,
	language NVARCHAR(50),
	data VARBINARY(MAX) NOT NULL
)

GO	

CREATE INDEX [NonCluteredIndex__type] on dbo.resources([type]) include([id],[guid],[language],[data]) with(fillfactor=80)
GO

CREATE INDEX [NonCluteredIndex__language] on dbo.resources([language]) include([id],[guid],[type],[data]) with(fillfactor=80);
GO

----- Translation memories
CREATE TABLE dbo.translation_memories(
	id INT IDENTITY NOT NULL PRIMARY KEY,
	guid UNIQUEIDENTIFIER NOT NULL UNIQUE,
	name NVARCHAR(200) NOT NULL UNIQUE,
	source_language NVARCHAR(50) NOT NULL,
	target_language NVARCHAR(50) NOT NULL,
	copyright NVARCHAR(MAX),
	description NVARCHAR(MAX),
	settings INT NOT NULL,
	creation_user NVARCHAR(50) NOT NULL,
	creation_date DATETIME NOT NULL,
	expiration_date DATETIME,
	fuzzy_indexes INT NOT NULL,
	last_recompute_date DATETIME,
	last_recompute_size INT,
	aligner_definition VARBINARY(MAX),
	fga_support INT NOT NULL,
	data_version INT NOT NULL,
	text_context_match_type INT NOT NULL,
	id_context_match bit not null
)

GO	

----- TM resources
CREATE TABLE dbo.tm_resources(
	tm_id INT NOT NULL CONSTRAINT FK_tr_t REFERENCES dbo.translation_memories(id) ON DELETE CASCADE,
	resource_id INT NOT NULL, --CONSTRAINT FK_tr_r REFERENCES dbo.resources(id) ON DELETE CASCADE,
CONSTRAINT PK_tr PRIMARY KEY CLUSTERED (
	tm_id,
	resource_id
)
)

GO	


----- Parameters
CREATE TABLE dbo.parameters(
	translation_memory_id INT NULL CONSTRAINT FK_p_tm REFERENCES dbo.translation_memories ON DELETE CASCADE,
	name NVARCHAR(50) NOT NULL,
	value NVARCHAR(MAX) NOT NULL
)

CREATE CLUSTERED INDEX p_main ON dbo.parameters(translation_memory_id, name)

INSERT INTO dbo.parameters(name, value) VALUES('FREQUENCYTOP', '1000')

GO	

----- Get parameter value
CREATE PROCEDURE dbo.get_parameter @name NVARCHAR(50), @tm_id INT = NULL AS
	SET NOCOUNT ON
	IF @tm_id IS NULL
		SELECT value FROM dbo.parameters WHERE translation_memory_id IS NULL AND name = @name
	ELSE
		SELECT value FROM dbo.parameters WHERE translation_memory_id = @tm_id AND name = @name
GO

----- Set parameter value
CREATE PROCEDURE dbo.set_parameter @name NVARCHAR(50), @value NVARCHAR(MAX), @tm_id INT = NULL AS
	SET NOCOUNT ON
	IF @tm_id IS NULL
	BEGIN
		DELETE FROM dbo.parameters WHERE translation_memory_id IS NULL AND name = @name
		INSERT INTO dbo.parameters (name, value) VALUES (@name, @value)
	END 
	ELSE BEGIN
		DELETE FROM dbo.parameters WHERE translation_memory_id = @tm_id AND name = @name
		INSERT INTO dbo.parameters (translation_memory_id, name, value) VALUES (@tm_id, @name, @value)
	END
GO

----- Resource handling
CREATE PROCEDURE dbo.get_resources @id INT = NULL, @type INT = NULL, @language NVARCHAR(50) = NULL, @tm_id INT = NULL AS
	SET NOCOUNT ON
	IF @id IS NOT NULL
		SELECT id, guid, type, language, data FROM dbo.resources WHERE id = @id
	ELSE IF @type IS NOT NULL AND @language IS NOT NULL
		SELECT id, guid, type, language, data FROM dbo.resources WHERE type = @type AND language = @language 
	ELSE IF @language IS NOT NULL
		IF @language = ''
			SELECT id, guid, type, language, data FROM dbo.resources WHERE language IS NULL
		ELSE
			SELECT id, guid, type, language, data FROM dbo.resources WHERE language = @language
	ELSE IF @tm_id IS NOT NULL AND @type IS NOT NULL
		SELECT r.id, r.guid, r.type, r.language, r.data 
			FROM dbo.resources r INNER JOIN dbo.tm_resources tr ON r.id = tr.resource_id
			WHERE tr.tm_id = @tm_id AND type = @type
	ELSE IF @tm_id IS NOT NULL
		SELECT r.id, r.guid, r.type, r.language, r.data 
			FROM dbo.resources r INNER JOIN dbo.tm_resources tr ON r.id = tr.resource_id
			WHERE tr.tm_id = @tm_id
	ELSE IF @type IS NOT NULL
		SELECT id, guid, type, language, data FROM dbo.resources WHERE type = @type
	ELSE
		SELECT id, guid, type, language, data FROM dbo.resources
GO

CREATE PROCEDURE dbo.get_tms_by_resource @resource_id INT AS
	SET NOCOUNT ON
	SELECT t.id, t.guid, t.name, t.source_language, t.target_language, 
		t.copyright, t.description, t.settings, t.creation_user, t.creation_date, 
		t.expiration_date, t.fuzzy_indexes, t.last_recompute_date, t.last_recompute_size, 
		t.fga_support, t.data_version, t.text_context_match_type, t.id_context_match
	FROM dbo.translation_memories t
	INNER JOIN dbo.tm_resources tr ON t.id = tr.tm_id
	WHERE tr.resource_id = @resource_id
GO

CREATE PROCEDURE dbo.add_resource @guid UNIQUEIDENTIFIER, @type INT, @language NVARCHAR(50) = NULL, @data VARBINARY(MAX) AS
	SET NOCOUNT ON
	INSERT INTO dbo.resources(type, language, data, guid) VALUES(@type, @language, @data, @guid)
	SELECT CAST(SCOPE_IDENTITY() AS INT)
GO

CREATE PROCEDURE dbo.delete_resource @id INT AS
	SET NOCOUNT ON
	DELETE FROM dbo.tm_resources WHERE resource_id = @id
	SET NOCOUNT OFF
	DELETE FROM dbo.resources WHERE id = @id
GO
	
CREATE PROCEDURE dbo.update_resource @id INT, @data VARBINARY(MAX) AS
	UPDATE dbo.resources SET data = @data WHERE id = @id
GO

CREATE PROCEDURE dbo.add_tm_resource @tm_id INT, @resource_id INT AS
	INSERT INTO dbo.tm_resources(tm_id, resource_id) VALUES(@tm_id, @resource_id)
GO

CREATE PROCEDURE dbo.delete_tm_resource @tm_id INT, @resource_id INT AS
	DELETE FROM dbo.tm_resources WHERE tm_id = @tm_id AND resource_id = @resource_id
GO

----- Translation memory handling
CREATE PROCEDURE dbo.get_tms @id INT = NULL, @guid UNIQUEIDENTIFIER = NULL, @name NVARCHAR(200) = NULL AS
	SET NOCOUNT ON
	IF @id IS NOT NULL
		SELECT id, guid, name, source_language, target_language, copyright, description, settings, creation_user, creation_date, expiration_date, 
			fuzzy_indexes, last_recompute_date, last_recompute_size, fga_support, data_version, text_context_match_type, id_context_match
			FROM dbo.translation_memories WHERE id = @id
	ELSE IF @guid IS NOT NULL
		SELECT id, guid, name, source_language, target_language, copyright, description, settings, creation_user, creation_date, expiration_date, 
			fuzzy_indexes, last_recompute_date, last_recompute_size, fga_support, data_version, text_context_match_type, id_context_match
			FROM dbo.translation_memories WHERE guid = @guid
	ELSE IF @name IS NOT NULL
		SELECT id, guid, name, source_language, target_language, copyright, description, settings, creation_user, creation_date, expiration_date, 
			fuzzy_indexes, last_recompute_date, last_recompute_size, fga_support, data_version, text_context_match_type, id_context_match
			FROM dbo.translation_memories WHERE name = @name
	ELSE
		SELECT id, guid, name, source_language, target_language, copyright, description, settings, creation_user, creation_date, expiration_date, 
			fuzzy_indexes, last_recompute_date, last_recompute_size, fga_support, data_version, text_context_match_type, id_context_match
			FROM dbo.translation_memories
GO	

CREATE PROCEDURE dbo.add_tm @guid UNIQUEIDENTIFIER, @name NVARCHAR(200), @src_lang NVARCHAR(50), @trg_lang NVARCHAR(50), @copyright NVARCHAR(MAX) = NULL, 
	@description NVARCHAR(MAX) = NULL, @settings INT, @cru NVARCHAR(50), @crd DATETIME, @expd DATETIME = NULL, @fuzzy_indexes INT, @fga_support INT,
	@data_version INT, @text_context_match_type INT, @id_context_match bit AS
	DECLARE @tm_id INT
	SET NOCOUNT ON
	INSERT INTO dbo.translation_memories(guid, name, source_language, target_language, copyright, description, settings, creation_user, 
		creation_date, expiration_date, fuzzy_indexes, fga_support, data_version, text_context_match_type, id_context_match) 
		VALUES(@guid, @name, @src_lang, @trg_lang, @copyright, @description, @settings, @cru, @crd, @expd, @fuzzy_indexes, @fga_support, @data_version, @text_context_match_type, @id_context_match)
	SET @tm_id = SCOPE_IDENTITY() 
	INSERT INTO dbo.parameters(translation_memory_id, name, value) VALUES(@tm_id, 'ADDTOMINSCORE', 0)
	INSERT INTO dbo.parameters(translation_memory_id, name, value) VALUES(@tm_id, 'MINHAVING1', 3)
	INSERT INTO dbo.parameters(translation_memory_id, name, value) VALUES(@tm_id, 'MINHAVING2', 5)
	INSERT INTO dbo.parameters(translation_memory_id, name, value) VALUES(@tm_id, 'MINHAVING4', 5)
	INSERT INTO dbo.parameters(translation_memory_id, name, value) VALUES(@tm_id, 'MINHAVING8', 3)
	SELECT @tm_id
GO

CREATE PROCEDURE dbo.delete_tm @id INT AS
	DELETE FROM dbo.translation_memories WHERE id = @id
GO

CREATE PROCEDURE dbo.update_tm @id INT, @guid UNIQUEIDENTIFIER, @name NVARCHAR(200), @src_lang NVARCHAR(50), @trg_lang NVARCHAR(50), 
	@copyright NVARCHAR(2000) = NULL, @description NVARCHAR(2000) = NULL, @settings INT, @cru NVARCHAR(50), @crd DATETIME, 
	@expd DATETIME = NULL, @fuzzy_indexes INT, @fga_support INT, @data_version INT AS
	UPDATE dbo.translation_memories SET guid = @guid, name = @name, source_language = @src_lang, target_language = @trg_lang, 
		copyright = @copyright, description = @description, settings = @settings, creation_user = @cru, creation_date = @crd, 
		expiration_date = @expd, fuzzy_indexes = @fuzzy_indexes, fga_support = @fga_support, data_version = @data_version
		WHERE id = @id
GO

CREATE PROCEDURE dbo.resolve_tm_guid @guid UNIQUEIDENTIFIER AS
	SELECT id FROM dbo.translation_memories WHERE guid = @guid
GO

CREATE PROCEDURE dbo.resolve_resource_guid @guid UNIQUEIDENTIFIER AS
	SELECT id FROM dbo.resources WHERE guid = @guid
GO

---- Table-valued type used for DTA searching 
CREATE TYPE dbo.Hashes AS TABLE 
(
	Hash bigint not null,
	PRIMARY KEY (Hash)
)
GO

---- Table-valued type used for TU retrieval
CREATE TYPE dbo.TuIDHashes AS TABLE 
(
    id integer not null,
	sourceHash bigint not null,
	targetHash bigint not null,
	PRIMARY KEY (id, sourceHash, targetHash)
)
GO

---- Table-valued type used for TU sid context
CREATE TYPE dbo.IdContexts AS TABLE 
(
	IdContext nvarchar(250) not null
)
GO

----- Table-valued type used for DTA index maintenance
CREATE TYPE dbo.TuFragmentIndexEntries AS TABLE
(
	TuId int not null,
	FragmentHash bigint not null,
	PRIMARY KEY (TuId, FragmentHash)
)
GO

----- Table-valued type used for bulk-get-by-id
CREATE TYPE dbo.TuIds AS TABLE
(
	TuId integer not null,
	PRIMARY KEY (TuId)
)
GO

-- Used for passing groups of tuids in a single parameter
CREATE TYPE dbo.GroupedTuIds AS TABLE
(
	TuId integer not null,
	GroupId integer not null
)
GO

CREATE TYPE dbo.TuIDFeatures AS TABLE
(
	TuId integer not null,
	Feature integer not null
)
GO

CREATE TYPE dbo.TUFilterParams AS TABLE
(
	[value] nvarchar(max) not null,
	[type] varchar(50) not null
)
GO

CREATE TYPE dbo.UpdateTuData2 AS TABLE
(
	id INT NOT NULL PRIMARY KEY, 
	source_hash BIGINT, 
	source_text NTEXT, 
	target_hash BIGINT, 
	target_text NTEXT,
	source_token_data VARBINARY(MAX),
	target_token_data VARBINARY(MAX),
	relaxed_hash BIGINT,
	source_tags VARBINARY(MAX),
	target_tags VARBINARY(MAX)
)
GO

CREATE TYPE dbo.FuzzyIndexData AS TABLE
(
	feature INT,
	translation_unit_id INT
)
GO

CREATE TYPE dbo.TuData3 AS TABLE -- TuData2 with Flags removed
(
    guid UNIQUEIDENTIFIER not null,
	id INT, 
	source_hash bigint,
	target_hash bigint,
	source_segment NTEXT, 
	target_segment NTEXT,
	creation_date DATETIME not null,
	creation_user NVARCHAR(50) not null,
	change_date DATETIME not null,
	change_user NVARCHAR(50) not null,
	last_used_date DATETIME not null, 
	last_used_user NVARCHAR(50) not null,
	usage_counter INT, 	
	source_token_data VARBINARY(MAX),
	target_token_data VARBINARY(MAX),
	alignment_data VARBINARY(MAX),
	align_model_date DATETIME,
	insert_date DATETIME,
	tokenization_sig_hash BIGINT not null,
	relaxed_hash BIGINT,
	serialization_version INT not null,
	source_tags VARBINARY(MAX),
	target_tags VARBINARY(MAX),
	importType int not null,
	format int,
	origin int,
	confirmationLevel int,
	PRIMARY KEY (guid)
)
GO 

CREATE TYPE dbo.TuData4 AS TABLE -- TuData3 with size changed from 50 to 255
(
    guid UNIQUEIDENTIFIER not null,
	id INT, 
	source_hash bigint,
	target_hash bigint,
	source_segment NTEXT, 
	target_segment NTEXT,
	creation_date DATETIME not null,
	creation_user NVARCHAR(255) not null,
	change_date DATETIME not null,
	change_user NVARCHAR(255) not null,
	last_used_date DATETIME not null, 
	last_used_user NVARCHAR(255) not null,
	usage_counter INT, 	
	source_token_data VARBINARY(MAX),
	target_token_data VARBINARY(MAX),
	alignment_data VARBINARY(MAX),
	align_model_date DATETIME,
	insert_date DATETIME,
	tokenization_sig_hash BIGINT not null,
	relaxed_hash BIGINT,
	serialization_version INT not null,
	source_tags VARBINARY(MAX),
	target_tags VARBINARY(MAX),
	importType int not null,
	format int,
	origin int,
	confirmationLevel int,
	PRIMARY KEY (guid)
)
GO 

CREATE TYPE dbo.TuFeatures AS TABLE
(
	guid UNIQUEIDENTIFIER not null,
	feature int, 
	length int,
	type int	
)
GO

CREATE TYPE dbo.TuStringAttributes AS TABLE
(
	guid UNIQUEIDENTIFIER not null,
	attribute_id int, 
	value NVARCHAR(MAX)
)
GO

CREATE TYPE dbo.TuDateAttributes AS TABLE
(
	guid UNIQUEIDENTIFIER not null,
	attribute_id int, 
	value DATETIME
)
GO

CREATE TYPE dbo.TuNumericAttributes AS TABLE
(
	guid UNIQUEIDENTIFIER not null,
	attribute_id int, 
	value int
)
GO



CREATE TYPE dbo.TuContexts AS TABLE
(
	guid UNIQUEIDENTIFIER not null,
	context1 BIGINT NOT NULL,
	context2 BIGINT NOT NULL,
	PRIMARY KEY (guid, context1, context2)
)
GO

CREATE TYPE dbo.TuIdContexts AS TABLE
(
	guid UNIQUEIDENTIFIER not null,
	idcontext nvarchar(250) not null,
	PRIMARY KEY (guid, idcontext)
)
GO

CREATE procedure [dbo].[tm_dropschema] @tmid int
as
begin

	declare @sql nvarchar(max) = '
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''fuzzy_index1_%%'') DROP TABLE dbo.fuzzy_index1_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''ff1_%%'') DROP TABLE dbo.ff1_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''fuzzy_index2_%%'') DROP TABLE dbo.fuzzy_index2_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''ff2_%%'') DROP TABLE dbo.ff2_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''fuzzy_index4_%%'') DROP TABLE dbo.fuzzy_index4_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''ff4_%%'') DROP TABLE dbo.ff4_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''fuzzy_index8_%%'') DROP TABLE dbo.fuzzy_index8_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''ff8_%%'') DROP TABLE dbo.ff8_%%

	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''picklist_attributes_%%'') DROP TABLE dbo.picklist_attributes_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''picklist_values_%%'') DROP TABLE dbo.picklist_values_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''date_attributes_%%'') DROP TABLE dbo.date_attributes_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''numeric_attributes_%%'') DROP TABLE dbo.numeric_attributes_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''string_attributes_%%'') DROP TABLE dbo.string_attributes_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''attributes_%%'') DROP TABLE dbo.attributes_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''translation_unit_contexts_%%'') DROP TABLE dbo.translation_unit_contexts_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''translation_unit_idcontexts_%%'') DROP TABLE dbo.translation_unit_idcontexts_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''translation_unit_fragments_%%'') DROP TABLE dbo.translation_unit_fragments_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''translation_unit_alignment_data_%%'') DROP TABLE dbo.translation_unit_alignment_data_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''translation_unit_last_search_%%'') DROP TABLE dbo.translation_unit_last_search_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''translation_units_%%'') DROP TABLE dbo.translation_units_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''vocab_src_%%'') DROP TABLE dbo.vocab_src_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''vocab_trg_%%'') DROP TABLE dbo.vocab_trg_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''trans_model_ints_%%'') DROP TABLE dbo.trans_model_ints_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''trans_model_floats_%%'') DROP TABLE dbo.trans_model_floats_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''vtranslation_units_flags_%%'') DROP VIEW dbo.vtranslation_units_flags_%%

	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''src_plain_segments_%%'') DROP TABLE dbo.src_plain_segments_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''trg_plain_segments_%%'') DROP TABLE dbo.trg_plain_segments_%%
	IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=''conc_segments_%%'') DROP TABLE dbo.conc_segments_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_dedup_context_hashes_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_dedup_context_hashes_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_dup_seg_hashes_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_dup_seg_hashes_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''create_hash_expansion_map_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.create_hash_expansion_map_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''process_additional_hashes_for_tu_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.process_additional_hashes_for_tu_%%


	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_count_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_count_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_attributes_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_attributes_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_attribute_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_attribute_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_attribute_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_attribute_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''rename_attribute_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.rename_attribute_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_attribute_picklist_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_attribute_picklist_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_picklist_value_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_picklist_value_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''rename_picklist_value_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.rename_picklist_value_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_picklist_value_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_picklist_value_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_attribute_value_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_attribute_value_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_attribute_values_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_attribute_values_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_orphan_contexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_orphan_contexts_%%

	--get_tus_ is absolete now 30.09.2019, delete in the future after version 10.0.4 gets deployed everywhere
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_ex_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_ex_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_ids_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_ids_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_ids_simple_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_ids_simple_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_tu_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_tu_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_tu_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_tu_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''update_tu_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.update_tu_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''update_tu_data_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.update_tu_data_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''update_tu_data_batch_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.update_tu_data_batch_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_tu_context_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_tu_context_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_tu_contexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_tu_contexts_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_contexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_contexts_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_contexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_contexts_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_tu_contexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_tu_contexts_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_features_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_features_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''recompute_frequencies_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.recompute_frequencies_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''clear_fuzzy_index_%% '' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.clear_fuzzy_index_%% 
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''fuzzy_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.fuzzy_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''fuzzy_search_concordance_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.fuzzy_search_concordance_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''exact_search_ex_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.exact_search_ex_%%	
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''duplicate_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.duplicate_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''duplicate_search_target_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.duplicate_search_target_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_feature_frequency_%%'' AND routine_type = ''FUNCTION'') DROP FUNCTION dbo.get_feature_frequency_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''resolve_tu_guid_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.resolve_tu_guid_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''resolve_attribute_guid_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.resolve_attribute_guid_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''resolve_picklistvalue_guid_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.resolve_picklistvalue_guid_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_tus_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_tus_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_tus_filtered_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_tus_filtered_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''tm_tdb_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.tm_tdb_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_tu_subsegmentindices_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_tu_subsegmentindices_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''update_tu_alignmentdata_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.update_tu_alignmentdata_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''dta_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.dta_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_by_ids_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_by_ids_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_ToAlign_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_ToAlign_%%
	
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''batch_exact_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.batch_exact_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''batch_duplicate_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.batch_duplicate_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_full_tus_by_ids_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_full_tus_by_ids_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_WithHashes_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_WithHashes_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''insert_tu_fragments_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.insert_tu_fragments_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''delete_tu_fragments_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.delete_tu_fragments_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''clear_alignment_data_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.clear_alignment_data_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_postdated_tu_count_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_postdated_tu_count_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_aligned_predated_tu_count_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_aligned_predated_tu_count_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''sp_get_unaligned_tu_count_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.sp_get_unaligned_tu_count_%%	
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''sp_GetAlignmentTimestampsByIds_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.sp_GetAlignmentTimestampsByIds_%%	
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''sp_GetAlignmentTimestampsPaginated_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.sp_GetAlignmentTimestampsPaginated_%%	

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_reindex_required_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_reindex_required_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_for_reindex_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_for_reindex_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_count_for_reindex_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_count_for_reindex_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_ex_f_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_ex_f_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_ex_f_attrsql_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_ex_f_attrsql_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_ft_indices_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_ft_indices_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''drop_ft_indices_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.drop_ft_indices_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_plain_segments_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_plain_segments_%%	
			
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tu_idcontexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tu_idcontexts_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''get_tus_idcontexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.get_tus_idcontexts_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_tu_idcontexts_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_tu_idcontexts_%%

	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''batch_fuzzy_search_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.batch_fuzzy_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''singleTU_fuzzy_search_%%'' AND routine_type = ''FUNCTION'') DROP FUNCTION dbo.singleTU_fuzzy_search_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''getFilteredTus_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.getFilteredTus_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''add_tu_batch_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.add_tu_batch_%%
	IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = ''sp_addUpdate_tu_lastSearch_%%'' AND routine_type = ''PROCEDURE'') DROP PROCEDURE dbo.sp_addUpdate_tu_lastSearch_%%
	'
	set @sql = replace(@sql, '%%', convert(Varchar(10), @tmid))
	exec sp_executesql @sql
end

GO



CREATE PROCEDURE [dbo].[tm_cleanup]
AS
BEGIN
	declare @sql nvarchar(max) = ''

	select @sql = @sql + 'exec tm_dropschema ' + convert(varchar(10), id) + ' '
	from (
		select distinct reverse(left(reverse(specific_name), charindex('_', reverse(specific_name)) -1)) id
		From INFORMATION_SCHEMA.routines where specific_name like '%\_%' escape '\'
		and isnumeric(reverse(left(reverse(specific_name), charindex('_', reverse(specific_name)) -1))) = 1
		union 
		select distinct reverse(left(reverse(table_name), charindex('_', reverse(table_name)) -1)) id
		From INFORMATION_SCHEMA.TABLES where table_name like '%\_%' escape '\'
		and isnumeric(reverse(left(reverse(table_name), charindex('_', reverse(table_name)) -1))) = 1		
	) x
    where id not in (select id from translation_memories)
	exec sp_executesql @sql
END

GO

