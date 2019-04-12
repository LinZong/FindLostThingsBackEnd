# FindLostThingsBackEnd
朝花夕拾 App后端

This backend app need to connect to mysql db. For preventing to storing sensitive db conn string into appsettings, 
you need to create secrets.json in a specified path the Secret Manager docs said.

For Windows : ```%APPDATA%\Microsoft\UserSecrets\<the-secret-guid-at-csproj>\```

For Linux/macOS : ```/.microsoft/<the-secret-guid-at-csproj>```

Add:

```
{
  "ConnectionStrings:MySQLConnectionString": "<dev-env-conn-str>",
  "ConnectionStrings:MySQLConnectionStringProd": "<prod-env-conn-str>"
}
```
