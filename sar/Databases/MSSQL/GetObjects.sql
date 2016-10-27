SELECT 
	name=o.name
	,type=o.type 
FROM sysobjects o
WHERE type in ('FN', 'IF', 'P', 'TF', 'TR', 'U', 'V') 
	AND name NOT LIKE 'sp[_]%' 
	AND name NOT LIKE 'fn[_]%' 
	AND name NOT LIKE 'sysdiagrams' 
UNION 
SELECT
	name=tt.name
	,type=o.type
FROM sysobjects o
INNER JOIN sys.table_types tt
	ON tt.type_table_object_id=o.id
WHERE o.type in ('TT')
ORDER BY o.type, o.name
