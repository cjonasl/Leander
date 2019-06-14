SELECT      sys.databases.name,  
            CONVERT(VARCHAR,SUM(size)*8/1024)+' MB' AS [Total disk space]  
FROM        sys.databases   
JOIN        sys.master_files  
ON          sys.databases.database_id=sys.master_files.database_id  
GROUP BY    sys.databases.name  
ORDER BY    sys.databases.name  