# Example IIS 10.0 configuration for Windows 10 and Windows Server 2016/2019
If you intend to run the application under IIS, you need to install 
[Application Request Routing 3](https://www.iis.net/downloads/microsoft/application-request-routing) extension for IIS 
first. Once it's installed, you have to enable ARR and reverse proxying from IIS management console:

![IIS 10.0: ARR 3 control panel location](https://static.emzi0767.com/misc/ctf/_rosetta/iis-01.png)

![IIS 10.0: ARR 3 proxy settings](https://static.emzi0767.com/misc/ctf/_rosetta/iis-02.png)

![IIS 10.0: enabling proxy and headers](https://static.emzi0767.com/misc/ctf/_rosetta/iis-03.png)

Next, you will have to enable `X-Forwarded-*` headers in URL Rewrite settings:

![IIS 10.0: URL Rewrite control panel location](https://static.emzi0767.com/misc/ctf/_rosetta/iis-04.png)

![IIS 10.0: URL Rewrite server variable settings](https://static.emzi0767.com/misc/ctf/_rosetta/iis-05.png)

![IIS 10.0: URL Rewrite server variables](https://static.emzi0767.com/misc/ctf/_rosetta/iis-06.png)

The variables you have to add are `HTTP_X_FORWARDED_HOST`, `HTTP_X_FORWARDED_PROTO`, and `HTTP_X_FORWARDED_SCHEMA`.

Next, you have to go to your site's configuration, to URL Rewrite settings:

![IIS 10.0: URL Rewrite site configuration](https://static.emzi0767.com/misc/ctf/_rosetta/iis-07.png)

From here, you have to create 2 rules: one to reverse proxy to the API server, and another to serve index.html to all 
404s. They should also be in that exact order. Let's set up the reverse proxy first:

![IIS 10.0: New rewrite rule](https://static.emzi0767.com/misc/ctf/_rosetta/iis-08.png)

![IIS 10.0: Reverse proxy rule](https://static.emzi0767.com/misc/ctf/_rosetta/iis-09.png)

![IIS 10.0: Reverse proxy endpoint](https://static.emzi0767.com/misc/ctf/_rosetta/iis-10.png)

Replace `localhost:5000` with your configured API server location. If your server does not use TLS, check "Enable SSL 
offloading". Once the rule is created, you can optionally rename it. In this example, I will name the rule "API".

![IIS 10.0: New rule](https://static.emzi0767.com/misc/ctf/_rosetta/iis-10.png)

![IIS 10.0: API rule](https://static.emzi0767.com/misc/ctf/_rosetta/iis-11.png)

Let's configure it, so that it functions properly. Double click it to edit it.

In here, you want to set the options as follows:

- **Match URL** 
   - **Requested URL:** Matches the Pattern
   - **Using:** Wildcards
   - **Pattern:** `api/*`
   - **Ignore case** checked
- **Conditions**
   - Remove all
- **Server variables**
   - `HTTP_X_FORWARDED_HOST` with value of `{HTTP_HOST}`, and "Replace existing value" checked.
   - `HTTP_X_FORWARDED_PROTO` with value of `https` or `http`, depending on whether you serve requests using TLS or not.
- **Action**
   - **Action type:** Rewrite
   - **Rewrite URL:** `http://<api host>/{R:0}`, where `http` is replaced with `https` if the API server uses TLS. 
     `<api host>` will be replaced with the host you entered earlier.
   - **Append query string** checked.
- **Stop processing of subsequent rules** checked.

![IIS 10.0: API rule details 1](https://static.emzi0767.com/misc/ctf/_rosetta/iis-12.png)

![IIS 10.0: API rule details 2](https://static.emzi0767.com/misc/ctf/_rosetta/iis-13.png)

Once that's done, press Apply on the right, and go back to the rules. Time to set up the SPA rule. Create a new blank 
rule, and name it "AngularSPA".

![IIS 10.0: New blank rule](https://static.emzi0767.com/misc/ctf/_rosetta/iis-14.png)

You want to set it up as follows:

- **Match URL**
   - **Requested URL:** Matches the pattern
   - **Using:** Wildcards
   - **Pattern:** `*`
   - **Ignore case** checked
- **Conditions** Create 2 new as follows:
   - `{REQUEST_FILENAME}`: **Check if input string:** Is Not a File
   - `{REQUEST_FILENAME}`: **Check if input string:** Is Not a Directory
- **Server variables** None
- **Action**
   - **Action type:** Rewrite
   - **Rewrite URL:** `index.html`
   - **Append query string** checked.
- **Stop processing of subsequent rules** checked.

![IIS 10.0: SPA rule details 1](https://static.emzi0767.com/misc/ctf/_rosetta/iis-17.png)

![IIS 10.0: SPA rule: not file](https://static.emzi0767.com/misc/ctf/_rosetta/iis-15.png)

![IIS 10.0: SPA rule: not directory](https://static.emzi0767.com/misc/ctf/_rosetta/iis-16.png)

![IIS 10.0: SPA rule details 2](https://static.emzi0767.com/misc/ctf/_rosetta/iis-18.png)

Once that's done, press Apply.

At this point, you should have a working configuration.
