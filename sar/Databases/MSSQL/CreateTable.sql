-- **************************************************
-- Author:		K Boronka
-- Create date: Oct 27, 2016
-- 
-- leveraged solutions propsed here:
-- http://stackoverflow.com/questions/21547/in-sql-server-how-do-i-generate-a-create-table-statement-for-a-given-table
-- **************************************************


declare @table varchar(100)
set @table = '%%TableName%%'

DECLARE @sql table
(
	s varchar(1000), 
	id int identity
)

DECLARE @Columns TABLE
(
	row int PRIMARY KEY IDENTITY(1,1),
	name nvarchar(256),
	type nvarchar(256),
	length int,
	nullable bit
)

INSERT INTO @Columns
	SELECT 
		name=t.COLUMN_NAME
		,type=t.DATA_TYPE
		,length=t.CHARACTER_MAXIMUM_LENGTH
		,nullable=COLUMNPROPERTY(OBJECT_ID(@table, 'U'), t.COLUMN_NAME, 'AllowsNull')
	FROM information_schema.columns t
	WHERE table_name = @table
	ORDER BY ORDINAL_POSITION
	
DECLARE @row int;
DECLARE @rows int;
DECLARE @name nvarchar(256)
DECLARE @type nvarchar(256)
DECLARE @length int
DECLARE @nullable bit
DECLARE @definition nvarchar(256)
DECLARE @delimiter nvarchar(1)

-- **************************************************
-- create table
-- **************************************************
insert into @sql(s) values ( 'IF NOT EXISTS (SELECT * FROM sysobjects WHERE name=''' + @table +''' AND xtype=''U'')' )
insert into @sql(s) values ( '  BEGIN' )
insert into @sql(s) values ( '    CREATE TABLE [' + @table + '] (' )

-- **************************************************
-- primary key
-- **************************************************
SET @rows = (SELECT COUNT(*) FROM @Columns)
SET @row = 1;
SET @delimiter = '';
WHILE (@row <= @rows)
	BEGIN
		SELECT 
			@name=c.name
			,@type=c.type
			,@length=c.length
			,@nullable=c.nullable
		FROM @Columns c 
		WHERE row=@row
		
		IF @type = 'sql_variant' SET @length = null;
		SET @definition = N'[' + @name + N'] [' + @type + ']' + coalesce('(' + cast(@length as varchar) + ')','')
		
		IF @nullable=1 SET @definition = @definition + ' NULL';
		IF @nullable=0 SET @definition = @definition + ' NOT NULL';
		
	
		IF exists (select id from syscolumns where object_name(id)=@table and name=@name and columnproperty(id, name, 'IsIdentity') = 1)
			SET @definition = @definition + N' ' + 'IDENTITY(' + cast(ident_seed(@table) as varchar) + ',' + cast(ident_incr(@table) as varchar) + ')'
		
		insert into @sql(s) values ( '      ' + @delimiter + @definition )
		SET @delimiter = ','
		SET @row = @row + 1
	END

-- **************************************************
-- primary key
-- **************************************************
DECLARE @PrimaryKeyName nvarchar(256);
SET @PrimaryKeyName = (SELECT constraint_name FROM information_schema.table_constraints WHERE table_name = @table and constraint_type='PRIMARY KEY')

IF @PrimaryKeyName is not null
	BEGIN
		DECLARE @PrimaryKeyColumns TABLE
		(
			row int PRIMARY KEY IDENTITY(1,1),
			name nvarchar(256)
		)
		
		INSERT INTO @PrimaryKeyColumns
			SELECT 
				name=p.COLUMN_NAME
			FROM information_schema.key_column_usage p
			WHERE table_name = @table AND CONSTRAINT_NAME LIKE N'PK_%'
			ORDER BY ordinal_position


		
		insert into @sql(s) values ('      ' + @delimiter + 'PRIMARY KEY (')

		SET @row = 1;
		SET @delimiter = '';
		WHILE (@row <= @rows)
		BEGIN
			SET @name = (SELECT name FROM @PrimaryKeyColumns WHERE row = @row)
			SET @definition = N'[' + @name + N'] '
			
			insert into @sql(s) values ( '        ' + @delimiter + @definition )
			SET @delimiter = ','
			SET @row = @row + 1
		END
		
		insert into @sql(s) values ( '      ' + ')' )
	END

insert into @sql(s) values ( '    )' )
insert into @sql(s) values ( '  END' )
insert into @sql(s) values ( '' )
-- **************************************************
-- end of create table
-- **************************************************

-- **************************************************
-- add missing columns
-- **************************************************
SET @rows = (SELECT COUNT(*) FROM @Columns)
SET @row = 1;
SET @delimiter = '';
WHILE (@row <= @rows)
	BEGIN
		SELECT 
			@name=c.name
			,@type=c.type
			,@length=c.length
			,@nullable=c.nullable
		FROM @Columns c 
		WHERE row=@row
		
		IF @type = 'sql_variant' SET @length = null;
		SET @definition = N'[' + @name + N'] ' + @type +  coalesce('(' + cast(@length as varchar) + ')','')
		IF exists (select id from syscolumns where object_name(id)=@table and name=@name and columnproperty(id, name, 'IsIdentity') = 1)
			SET @definition = @definition + N' ' + 'IDENTITY(' + cast(ident_seed(@table) as varchar) + ',' + cast(ident_incr(@table) as varchar) + ')'
		
		insert into @sql(s) values ( '' )
		insert into @sql(s) values ( 'IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N''' + @table + ''') AND name = ''' + @name + ''')' )
		insert into @sql(s) values ( '  BEGIN' )
		insert into @sql(s) values ( '    ALTER TABLE ' + @table )
		insert into @sql(s) values ( '      ADD ' + @definition )
		insert into @sql(s) values ( '  END' )
		
		SET @row = @row + 1
	END


-- **************************************************
-- add foreign keys
-- **************************************************
DECLARE @FK_Name VARCHAR(100)
DECLARE @FK_ColumnName VARCHAR(50)
DECLARE @FK_TableName VARCHAR(100)
DECLARE @RF_TableName VARCHAR(100)
DECLARE @RF_ColumnName VARCHAR(50)
DECLARE @SC_Name VARCHAR(10)
DECLARE @UpdateAction VARCHAR(16)
DECLARE @DeleteAction VARCHAR(16)

DECLARE @ForeignKeys Table
(
	row int PRIMARY KEY IDENTITY(1,1),
	FK_Name VARCHAR(100),
	FK_TableName VARCHAR(100),
	FK_ColumnName VARCHAR(50),
	RF_TableName VARCHAR(100),
	RF_ColumnName VARCHAR(50),
	SC_Name VARCHAR(10),
	UpdateAction VARCHAR(16),
	DeleteAction VARCHAR(16)
)

INSERT INTO @ForeignKeys
	SELECT 
		f.name AS FK_Name, 
		OBJECT_NAME(f.parent_object_id) AS FK_TableName, 
		COL_NAME(fc.parent_object_id, fc.parent_column_id) AS FK_ColumnName, 
		OBJECT_NAME (f.referenced_object_id) AS RF_TableName, 
		COL_NAME(fc.referenced_object_id,fc.referenced_column_id) AS RF_ColumnName, 
		schema_name(f.schema_id) as SC_ID, 
		update_referential_action_desc AS UpdateAction, 
		delete_referential_action_desc AS DeleteAction
	FROM sys.foreign_keys AS f 
	INNER JOIN sys.foreign_key_columns AS fc 
		ON f.OBJECT_ID = fc.constraint_object_id 
	WHERE OBJECT_NAME(f.parent_object_id) = @table ORDER BY f.name
	
SET @rows = (SELECT COUNT(*) FROM @ForeignKeys)
SET @row = 1;
WHILE (@row <= @rows)
	BEGIN
		Select 
			@FK_Name=FK_Name
			,@FK_TableName=FK_TableName
			,@FK_ColumnName=FK_ColumnName
			,@RF_TableName=RF_TableName
			,@RF_ColumnName=RF_ColumnName
			,@SC_Name=SC_Name
			,@UpdateAction=UpdateAction
			,@DeleteAction=DeleteAction
		FROM @ForeignKeys where row=@row
		
		SET @definition = '['+ @SC_Name +'].['+ @FK_TableName +'] WITH ' + 'CHECK ADD CONSTRAINT ['+@FK_Name+'] FOREIGN KEY(['+@FK_ColumnName+']) REFERENCES ['+@SC_Name+'].['+@RF_TableName+'] (['+@RF_ColumnName+'])'
	
		If @UpdateAction != 'NO_ACTION' 
			SET @definition = @definition + ' ON UPDATE ' + @UpdateAction
			
		If @DeleteAction != 'NO_ACTION'
			SET @definition = @definition + ' ON DELETE ' + @DeleteAction
		
		insert into @sql(s) values ( '' )
		insert into @sql(s) values ( '' + 'IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N''['+ @SC_Name +'].['+ @FK_Name +']'') AND OBJECTPROPERTY(o.object_id, N''IsForeignKey'') = 1)' )
		insert into @sql(s) values ( '  ' + 'BEGIN' )
		insert into @sql(s) values ( '    ' + 'ALTER TABLE ' +  @definition )
		insert into @sql(s) values ( '  ' + 'END' )
		SET @row = @row + 1
	END


SELECT s FROM @sql WHERE s is not null order by id