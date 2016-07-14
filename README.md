[![Build status](https://ci.appveyor.com/api/projects/status/duid2ycm16mgmdb3/branch/master?svg=true)](https://ci.appveyor.com/project/rms81/identityserver3-contrib-nhibernate/branch/master)

# Nhibernate Persistence Layer for IdentityServer3 #

Just use the extension method RegisterNhibernateStores with an Nhibernate Session Factory:

    var factory = new IdentityServerServiceFactory();
    factory.RegisterNhibernateStores(new NhibernateServiceOptions(nhSessionFactory)
    {
       RegisterOperationalServices = true,
       RegisterConfigurationServices = true
    });

A way of getting the SessionFactory. Use MappingHelper.GetNhibernateServicesMappings() to get the mappings.

    private static ISessionFactory GetNHibernateSessionFactory()
    {
       var connString = ConfigurationManager.ConnectionStrings["IdSvr3Config"];

       var sessionFactory = Fluently.Configure()
           .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connString.ToString())
              .ShowSql()
              .FormatSql())
           .Mappings(
               m => m.AutoMappings.Add(MappingHelper.GetNhibernateServicesMappings(true, true)))
           .ExposeConfiguration(cfg =>
            {
                SchemaMetadataUpdater.QuoteTableAndColumns(cfg);
                BuildSchema(cfg);
            })
            .BuildSessionFactory();

        return sessionFactory;
    }

To auto create the tables:

    private static void BuildSchema(Configuration cfg)
    {
       new SchemaUpdate(cfg).Execute(false, true);
    }  
