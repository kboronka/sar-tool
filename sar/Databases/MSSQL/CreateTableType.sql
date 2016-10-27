-- **************************************************
-- Author:		K Boronka
-- Create date: Oct 27, 2016
-- **************************************************

declare @table varchar(100)
set @table = '%%TableTypeName%%'

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
		name=c.name
		,type=t.name
		,length=c.prec
		,nullable=c.isnullable
	FROM sys.table_types tt
	LEFT JOIN syscolumns c ON c.id=tt.type_table_object_id
	LEFT JOIN systypes t ON t.xusertype=c.xtype
	WHERE tt.name=@table
	ORDER BY c.colid
	
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
insert into @sql(s) values ( N'IF NOT EXISTS (SELECT * FROM sys.table_types WHERE name=''' + @table +''')' )
insert into @sql(s) values ( N'  BEGIN' )
insert into @sql(s) values ( N'    CREATE TYPE [' + @table + '] AS TABLE (' )

-- **************************************************
-- columns
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
		
		IF @type NOT LIKE 'var%' AND @type NOT LIKE 'nvar%' SET @length = null;
		SET @definition = N'[' + @name + N'] [' + @type + ']' 
		
		IF @length>=0 SET @definition = @definition + '(' + cast(@length as varchar) + ')';
		IF @length=-1 SET @definition = @definition + '(max)';
		
		IF @nullable=1 SET @definition = @definition + ' NULL';
		IF @nullable=0 SET @definition = @definition + ' NOT NULL';
		
	
		IF exists (select id from syscolumns where object_name(id)=@table and name=@name and columnproperty(id, name, 'IsIdentity') = 1)
			SET @definition = @definition + N' ' + 'IDENTITY(' + cast(ident_seed(@table) as varchar) + ',' + cast(ident_incr(@table) as varchar) + ')'
		
		insert into @sql(s) values ( '      ' + @delimiter + @definition )
		SET @delimiter = ','
		SET @row = @row + 1
	END


insert into @sql(s) values ( '    )' )
insert into @sql(s) values ( '  END' )
insert into @sql(s) values ( '' )
-- **************************************************
-- end of create table
-- **************************************************


SELECT s FROM @sql WHERE s is not null order by id