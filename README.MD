# RosettaCTF
A simple CTF platform for quicktly setting up your own events, built using ASP.NET Core 3.1 and Angular 10. It's quick 
and easy to set up (all you need to do is fill appropriate values in relevant config files), is fully Cloudflare- and 
Docker-compatible.

Rosetta uses PostgreSQL as its main storage engine, and Redis as its cache storage provider, however it's possible and 
relatively easy to implement other providers; all you have to do is implement provided interfaces and flag your 
assemblies with appropriate attributes.

## Setup
Rosetta provides 2 different ways of authenticating users - via OAuth2, and/or via username/password authentication, 
with optional TOTP 2FA support. By default only Discord has a fully-featured built-in OAuth2 provider, however, the 
instance operator can configure any number of custom providers via config.json.

To enable OAuth, set `authentication / oauth / enable` to `true`. If OAuth is enabled, the `tokenKey` under 
`authentication / oauth` should be set to a strong random string, as it will be used for token storage. 

To enable local username/password authentication, set `authentication / localLogin` to `true`.

To use Discord as an authentication provider, create an application on [Discord developers website](https://discord.com/developers/applications), 
and paste the client ID and secret into appropriate places in the config (or supply them via other means, such as 
environment variables or commandline args). In the OAuth2 tab, you should add a redirect URL to the application. It 
should have a path of `/session/callback` (so the URL should look like `https://myrosettactfinstance.xyz/session/callback`). 
You should also set `guildId` to your event server's ID. Rosetta will only authorize users who are in your specified 
server to take part in the event.

Next step is configuring Rosetta's JWT provider. You should provide values for the `tokenIssuer` and `tokenKey` fields 
under `authentication`. Your key should be sufficiently strong to be considered secure, as it will be used to sign 
tokens. A weak key might lead to users impersonating other users.

Next up is HTTP configuration. You can specify multiple HTTP listen endpoints, each as a separate item in the listen 
array. If you want an endpoint to use TLS, you should set `useSsl` to `true`, and supply the path to the certificate 
file in PKCS#12 format, and a file containing the password to it. It is also configure proxy settings in here, if the 
API server is deployed behind one, such as nginx reverse proxy or Cloudflare.

For cache and database, options are provider-dependent. You should configure them appropriately for your providers.

Lastly, `eventConfiguration` should lead to a YAML file with all the necessary challenge definitions. The YAML file 
should contain 2 documents, the first one should be event configuration (name, start, end, organizers, etc.), whereas 
the second one should contain all event definitions.

See `config.json.example` and `event.yml.example` in the root of this repository for example configuration files.

### Configuring via JSON file
By default, Rosetta will attempt to read any file called `config.json` in its working directory, and parse its values 
as configuration. 

The name and location of this file can be overriden via environment variables, by setting 
`ROSETTACTF__JSONCONFIGURATION` (note the double underscore) variable to a valid JSON file path. For example, to load 
JSON configuration from a Docker secret called `rosetta-config`, you would specify the variable like so: 
`ROSETTACTF__JSONCONFIGURATION=/run/secrets/rosetta-config`.

The location can also be overriden via commandline, by setting `--jsonConfiguration=path`.

### Configuring via YAML file
Much like with JSON, Rosetta will attempt to read and parse `config.yml` from its working directory as configuration.

Much like with JSON configuration, its name and location can be overriden via `ROSETTACTF__YAMLCONFIGURATION` 
environment variable or `--yamlConfiguration` commandline option.

JSON and YAML configuration can be used at the same time, however some caveats apply.

### Configuring via environment variables
All of the configuration settings listed in `config.json.example` can also be provided via environment variables. 
Rosetta will automatically parse any variables with names starting with `ROSETTACTF__` (note the double underscore). 
Double underscores will be treated as path separators, meaning that in order to set value for `discord / clientId`, 
you have to create a variable called `ROSETTACTF__DISCORD__CLIENTID` (again, note double underscores), and set its 
value appropriately.

To set value for list items, such as `http / listen / * / address`, you use the index as if it was a property name. 
Remember that indexes are 0-based, so to provide the bind address for the first endpoint, you would specify it via 
variable called `ROSETTACTF__HTTP__LISTEN__0__ADDRESS`.

### Configuring via commandline options
Similarly to environment variables, Rosetta accepts settings input from commandline, using `:` as path separator. This 
means that in order to set `discord / clientId`, you would pass `--discord:clientId=my_client_id` via commandline. 
Similarly to environment variables, list items use index as property name, so to set bind address of first listen 
endpoint (`http / listen / 0 / address`), you would specify `--http:listen:0:address=localhost`.

### appsettings.json and appsettings.\*.json
Default ASP.NET Core configuration facilities load and parse this file as the first configuration source. It is 
therefore possible to also use it as a configuration source for all the options from `config.json.example`. 
Furthermore, it is possible to specify the JSON and YAML configuration paths, by setting 
`"jsonConfiguration": "path"` and/or `"yamlConfiguration": "path"` in the root object of the file.

It is also possible to set different configuration options for all 3 types of environments supported by ASP.NET Core, 
by using the `appsettings.Environment.json` file (where `Environment` is one of `Development`, `Staging`, or 
`Production`). The values from `appsettings.Environment.json` will be merged with `appsettings.json` at runtime, and 
any values specified in environment-specific version of the file will overwrite the values specified in the base file.

Please note that caveats apply.

### Configuration priority
All 5 configuration sources can be used at the same time, however some caveats apply and should be kept in mind. 

Firstly, the order of configuration loading. When the application starts, the first loaded configuration is 
`appsettings.json` and `appsettings.Environment.json` (where `Environment` is the current environment, see the 
relevant section of this readme for more details). Next, Rosetta will read any configuration from environment 
variables and commandline options. From these sources, the locations and names of JSON and YAML configuration are 
determined, if applicable. Locations defined in commandline options will take precedence over those defined in 
environment variables, which in turn take precedence over `appconfig.json` and `appconfig.Environment.json`. Next, 
Rosetta will attempt to load any configuration from the specified JSON and YAML files. Values from these sources are 
then merged with the rest.

Second thing to keep in mind is the configuration priority. The final value priority is as follows (from lowest to 
highest priority):

1. `appsettings.json`
2. `appsettings.Environment.json`
3. JSON configuration file (defaults to `config.json` in working directory; skipped if not found)
4. YAML configuration file (defaults to `config.yml` in working directoryl skipped if not found)
5. Environment variables prefixed with `ROSETTACTF__`
6. Commandline options

## Setting up challenges
As per the configuration, you will have to create event configuration as a 2-document YAML file. An example file is 
provided as `event.yml.example` in the root of the repository. Remember that YAML is a very sensitive format, and it's 
highly recommended you use an editor with proper support for it (such as Notepad++ or Visual Studio Code).

For more information on editing YAML files, see [YAML page on Wikipedia](https://en.wikipedia.org/wiki/YAML), 
[The official YAML website](https://yaml.org/), and a 
[website about multiline strings in YAML](https://yaml-multiline.info/).

## Deployment
Once you have configured your event, you can proceed to deployment. The process consists of 2 main parts. You have to 
configure and run the API server, and then you have to deploy the SPA files to a place where static content is served 
from.

Next, you have to configure your HTTP server, such that it passes any request to `/api/*` to the API server **without 
rewriting the path**, and any other request should be served as a file, unless it does not exist, in which case 
`index.html` should be returned.

### Example nginx configuration for Linux
Please note that while nginx is recommended, Apache should also work.

```
# HTTP/80
# Redirect this to HTTPS/443
server {
        # Listen for HTTP connections on port 80
        listen      80;
        listen [::]:80;

        # Set the server name
        server_name myrosettactfinstance.xyz;

        # Permanent redirect to HTTPS
        return 301 https://$server_name$request_uri;
}

# HTTPS/443
# Use HTTPS with HTTP/2 and HSTS
server {
        # Listen for HTTPS connections on port 443
        listen      443 ssl http2;
        listen [::]:443 ssl http2;

        # Set the server name
        server_name myrosettactfinstance.xyz;

        # Include certificate configuration
        include snippets.d/ssl-letsencrypt.conf;

        # Set content root and content index
        root /var/www/myrosettactfinstance.xyz/pub_html;
        index index.html;

        # Reverse proxy
        # Proxy all other requests to another server
        location /api {
                proxy_pass          https://localhost:5000;
                proxy_set_header    X-Real-IP           $remote_addr;
                proxy_set_header    Host                $http_host;
                proxy_set_header    X-Forwarded-For     $proxy_add_x_forwarded_for;
                proxy_set_header    X-Forwarded-Proto   $scheme;
                proxy_set_header    X-Forwarded-Scheme  $scheme;
                proxy_set_header    X-Forwarded-Host    $http_host;

                proxy_redirect off;
                proxy_buffering off;
        }

        # Handle all other requests as usual, but instead of returning 404s, return index.html
        location / {
                try_files $uri $uri/ /index.html;
        }
}
```

### Example IIS 10.0 configuration for Windows 10 and Windows Server 2016/2019
[Instructions on separate page](SETUP-IIS.MD)

## Other considerations
On startup, Rosetta transfers all challenges from your event configuration file (default: `event.yml`) to the 
persistent storage that is database for integrity purposes. This means that if you want to update challenges, you will 
have to either clear the relevant tables in your database (for default postgres provider that is `challenges`, 
`challenge_categories`, `challenge_hints`, and `challenge_attachments`) to let Rosetta reinstall the challenges on 
next startup, or manually reflect the changes in the database. Keep in mind that in case of relational databases, 
there will be foreign key constraints between tables (which will also involve solve table, `solves` in the default 
provider), so be careful with this.

## Extending RosettaCTF
It is possible to implement your own cache, database, and OAuth providers. You can check the existing providers for 
reference on how to do so.

Your project should reference `RosettaCTF.Abstractions`, which provides all the necessary abstractions, common types, 
and utilities which are needed to implement a Rosetta component.

The assembly should be tagged with `CacheProviderAttribute`, `DatabaseProviderAttribute`, or `OAuthProviderAttribute` 
respectively for cache, database, and OAuth providers. Furthermore, the assembly should provide a class which 
implements `ICacheServiceInitializer`, `IDatabaseServiceInitializer`, or `IOAuthProviderServiceInitializer` interface, 
respective for the type of component, which is default-constructible. The class has methods for registering and 
initializing the provider, to allow for performing any startup tasks.

It is typically expected that a cache provider will provide implementations for `ICtfChallengeCacheRepository`, 
`IOAuthStateRepository`, and `IMfaStateRepository`. A database provider should typically provide `IUserRepository`, 
`ICtfChallengeRepository`, and `IMfaRepository`. This, however, is not strongly-defined, and these types can be 
provided by any of these components. The general idea is that cache should provide short-lived data, such as states 
and current point counts, whereas a database should be a persistent storage.

An OAuth provider should provide an implementation of `IOAuthProvider`.

The built assemblies can be dropped into RosettaCTF's API server working directory, along with any dependencies. They 
should be detected and loaded automatically. However, that might not always work, due to how .NET Core handles 
dependencies. In such event, a project can be created in respective directory, and then RosettaCTF.API project can be 
built. The API project is set up such that it automatically references any projects in `src/cache`, `src/database`, 
and `src/oauth`. This adds those projects to the dependency tree, which causes them to be automatically deployed with 
all their dependencies.

## Building RosettaCTF
RosettaCTF consists of 2 parts: the API server (RosettaCTF.API project), and the SPA webapp (RosettaCTF.UI project). 
To build the former, .NET Core SDK 3.1 (version 3.1.401 or better) is required. To build the latter, node.js 10.0.0 or 
better is required.

By default, building the API project will build the SPA as well. This can take quite a while, so in order to omit 
building SPA, you can specify `/p:BuildSpa=False` flag.

To build the SPA separately from the API project, run `npm run prod`. This will build a deployment-ready production 
version of the SPA. Do not forget to restore packages by doing `npm install` before building.

## Support me
Lots of effort went into making this software.

If you feel like I'm doing a good job, or just want to throw money at me, you can do so through any of the following:
- [Patreon](https://www.patreon.com/emzi0767)
- [PayPal](https://paypal.me/Emzi0767/5USD)

## Other questions
If you have other questions or would like to talk in general, feel free to visit my Discord server.

[![Emzi's Central Dispatch](https://discordapp.com/api/guilds/207879549394878464/embed.png?style=banner1)](https://discord.gg/rGKrJDR)
