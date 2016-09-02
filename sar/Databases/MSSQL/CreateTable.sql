-- http://stackoverflow.com/questions/21547/in-sql-server-how-do-i-generate-a-create-table-statement-for-a-given-table
declare @table varchar(100)
 set @table = '%%TableName%%'
 declare @sql table(s varchar(1000), id int identity)

 insert into @sql(s) values ('if not exists (select * from sysobjects where name=''' + @table +''' and xtype=''U'')')
 insert into @sql(s) values ('CREATE TABLE [' + @table + '] (')
 insert into @sql(s)

 select 
     '  ['+column_name+'] ' + data_type + case when data_type = 'sql_variant' then '' else coalesce('(' + cast(character_maximum_length as varchar) + ')','') end + ' ' +
     case when exists ( 
         select id from syscolumns
         where object_name(id)=@table
         and name=column_name
         and columnproperty(id,name,'IsIdentity') = 1 
     ) then
         'IDENTITY(' + 
         cast(ident_seed(@table) as varchar) + ',' + 
         cast(ident_incr(@table) as varchar) + ')'
     else ''
     end + ' ' +
     ( case when IS_NULLABLE = 'No' then 'NOT ' else '' end ) + 'NULL ' + 
     coalesce('DEFAULT '+COLUMN_DEFAULT,'') + ','

 from information_schema.columns where table_name = @table
 order by ordinal_position

 declare @pkname varchar(100) -- primary key
 select @pkname = constraint_name from information_schema.table_constraints
 where table_name = @table and constraint_type='PRIMARY KEY'

 if ( @pkname is not null ) begin
     insert into @sql(s) values('  PRIMARY KEY (')
     insert into @sql(s) select '   ['+COLUMN_NAME+'],' from information_schema.key_column_usage where constraint_name = @pkname order by ordinal_position
     update @sql set s=left(s,len(s)-1) where id=@@identity -- remove trailing comma
     insert into @sql(s) values ('  )')
 end
 else begin
     update @sql set s=left(s,len(s)-1) where id=@@identity
 end

 insert into @sql(s) values( ')' )

 DECLARE @FK_Name VARCHAR(100)
 DECLARE @FK_TableName VARCHAR(100)
 DECLARE @FK_ColumnName VARCHAR(50)
 DECLARE @RF_TableName VARCHAR(100)
 DECLARE @RF_ColumnName VARCHAR(50)
 DECLARE @SC_Name VARCHAR(10)
 DECLARE @UpdateAction VARCHAR(16)
 DECLARE @DeleteAction VARCHAR(16)
 DECLARE @NoCheck Char(2)
 DECLARE @TEMP NVARCHAR(MAX)
 DECLARE @vCount Int
 DECLARE @vNumDBs Int
 DECLARE @DB_NAME VARCHAR(500)
 DECLARE @NoCheckVal VARCHAR(2)
 DECLARE @vFKeyList Table
 (
     SLID INT NOT NULL IDENTITY(1, 1),
     FK_Name VARCHAR(100),
     FK_TableName VARCHAR(100),
     FK_ColumnName VARCHAR(50),
     RF_TableName VARCHAR(100),
     RF_ColumnName VARCHAR(50),
     SC_Name VARCHAR(10),
     UpdateAction VARCHAR(16),
     DeleteAction VARCHAR(16)
 )
         
 INSERT INTO @vFKeyList SELECT f.name AS FK_Name, OBJECT_NAME(f.parent_object_id) AS FK_TableName, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS FK_ColumnName, OBJECT_NAME (f.referenced_object_id) AS RF_TableName, COL_NAME(fc.referenced_object_id,fc.referenced_column_id) AS RF_ColumnName, schema_name(f.schema_id) as SC_ID, update_referential_action_desc AS UpdateAction, delete_referential_action_desc AS DeleteAction FROM sys.foreign_keys AS f INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id WHERE OBJECT_NAME(f.parent_object_id) = @table ORDER BY f.name

 Set @vNumDBs = @@RowCount
 Set @vCount = 1
 If @NoCheckVal is null
 BEGIN
 SET @NoCheck=''
 END
 ELSE
 SET @NoCheck=@NoCheckVal

 While @vCount <= @vNumDBs

 Begin
 Select @FK_Name=FK_Name, @FK_TableName=FK_TableName, @FK_ColumnName=FK_ColumnName, @RF_TableName=RF_TableName, @RF_ColumnName=RF_ColumnName,@SC_Name=SC_Name, @UpdateAction=UpdateAction, @DeleteAction=DeleteAction FROM @vFKeyList where SLID= @vCount

 SET @TEMP='ALTER TABLE ['+ @SC_Name +'].['+ @FK_TableName +'] WITH ' + @NoCheck +'CHECK ADD CONSTRAINT ['+@FK_Name+'] FOREIGN KEY(['+@FK_ColumnName+']) REFERENCES ['+@SC_Name+'].['+@RF_TableName+'] (['+@RF_ColumnName+'])'
 If @UpdateAction != 'NO_ACTION' SET @TEMP = @TEMP + ' ON UPDATE ' + @UpdateAction
 If @DeleteAction != 'NO_ACTION' SET @TEMP = @TEMP + ' ON DELETE ' + @DeleteAction
 insert into @sql(s) values (@TEMP)

 SET @TEMP='ALTER TABLE ['+ @SC_Name +'].['+ @FK_TableName +'] CHECK CONSTRAINT ['+@FK_Name+']'
 insert into @sql(s) values (@TEMP)

 Set @vCount = @vCount + 1
 End


 select s from @sql order by id


