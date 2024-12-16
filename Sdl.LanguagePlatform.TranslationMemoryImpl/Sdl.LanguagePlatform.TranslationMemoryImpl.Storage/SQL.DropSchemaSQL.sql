IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'tm_cleanup' AND routine_type = 'PROCEDURE') exec tm_cleanup
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='parameters') DROP TABLE dbo.parameters
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='tm_resources') DROP TABLE dbo.tm_resources
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='translation_memories') DROP TABLE dbo.translation_memories
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='resources') DROP TABLE dbo.resources
GO

IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'get_parameter' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.get_parameter
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'set_parameter' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.set_parameter

IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'get_resources' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.get_resources
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'get_tms_by_resource' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.get_tms_by_resource
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'add_resource' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.add_resource
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'delete_resource' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.delete_resource
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'update_resource' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.update_resource

IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'add_tm_resource' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.add_tm_resource
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'delete_tm_resource' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.delete_tm_resource

IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'get_tms' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.get_tms
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'add_tm' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.add_tm
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'delete_tm' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.delete_tm
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'update_tm' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.update_tm

IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'resolve_tm_guid' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.resolve_tm_guid
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'resolve_resource_guid' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.resolve_resource_guid
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'tm_dropschema' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.tm_dropschema
IF EXISTS (SELECT routine_name FROM information_schema.routines WHERE routine_name = 'tm_cleanup' AND routine_type = 'PROCEDURE') DROP PROCEDURE dbo.tm_cleanup

IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'Hashes') DROP TYPE dbo.Hashes
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuFragmentIndexEntries') DROP TYPE dbo.TuFragmentIndexEntries
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuIds') DROP TYPE dbo.TuIds
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'GroupedTuIds') DROP TYPE dbo.GroupedTuIds
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuIDFeatures') DROP TYPE dbo.TuIDFeatures
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuIDHashes') DROP TYPE dbo.TuIDHashes
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'IdContexts') DROP TYPE dbo.IdContexts
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TUFilterParams') DROP TYPE dbo.TUFilterParams
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'UpdateTuData2') DROP TYPE dbo.UpdateTuData2
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'FuzzyIndexData') DROP TYPE dbo.FuzzyIndexData


IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuData4') DROP TYPE dbo.TuData4
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuData3') DROP TYPE dbo.TuData3
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuData2') DROP TYPE dbo.TuData2
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuData') DROP TYPE dbo.TuData
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuFeatures') DROP TYPE dbo.TuFeatures
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuStringAttributes') DROP TYPE dbo.TuStringAttributes
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuDateAttributes') DROP TYPE dbo.TuDateAttributes
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuNumericAttributes') DROP TYPE dbo.TuNumericAttributes
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuContexts') DROP TYPE dbo.TuContexts
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'TuIdContexts') DROP TYPE dbo.TuIdContexts

GO
