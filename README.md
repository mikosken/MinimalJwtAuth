# Minimal JWT Auth Example

This is a minimal example implementation of .Net-identity with JSON Web Token
based access control for REST-APIs. 

The code is fairly extensively commented, and some possible suggestions for
extensions and possible useage are also in comments.

## In depth

One of the simpler ways to secure an API in dotNet is to use the Identity package
with JSON Web Token bearers.

JWTs are Base64-encoded strings with a Header, Payload, and Signature.

The header contains information such as the algorithm used for signing the JWT.
The signature is generated from the base64-encoded header and payload in combination
with a secret key. This allows the issuer/server to verify that the token hasn't
been tampered with.
The payload contains the data we wish to send to the user. In the case of
using JWTs for auth purposes the payload may contain such data as username,
assigned claims and roles, and token expiry date.

To access a protected resource, first the user must login. Upon successful login
a ViewModel is returned to the user, containing at least a JWT.
The second step for the user is to then attach the JWT string as a header in
an API call to the protected resource, in the format:

```
Authorization: Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoidHJ1ZSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJqb2huQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiam9obkBleGFtcGxlLmNvbSIsIm5iZiI6MTY1MjAzMzY2NSwiZXhwIjoxNjUyMTIwMDY1fQ.65PwkhVizr79EU425V5h496cUos-IoZPHCDn516VOAlyRJNFWoP9p-q6Uw8eQpW4BPqsKRy699iM0V0-WPy6gw
```

Where "Authorization" is the header key, and the value is the entire string
"Bearer eyJhb...Py6gw".

If the user has access to the requested resource, and the header has been sent
correctly, the call should return a successful response.

Try copying and pasting the string eyJhb...Py6gw from above into the JWT debugger
at [JWT.io](https://jwt.io/) to see how the undecoded data is formatted.

## Example useage using swagger

To try out the functionality of this project, follow these steps.

First clone and run the project using the instructions in
[Getting Started](#Getting-Started) below.

Once you have the project running, if it doesn't automatically open your browser
at the built-in swagger page, open https://localhost:<port>/swagger/index.html

You can find the correct port number in the console where you ran
"dotnet watch run". Look for a locahost URL starting with **https** and
copy-paste that into your browser.

You should arrove at a page similar to this:

[Image 1]

Expand the section **/api/auth/register**, click the button **Try it out**, and
fill out the text box similar to this before clicking **Execute**.

[Image 2]

You should receive a response 201: Created. At this point you can try to log in
by expanding the section **/api/auth/login**, click the button **Try it out**,
and fill out the text box as before, and finally clicking **Execute**.

[Image 3]

You should receive a response 200: Success with a token in the request body.
Copy the long string inside the citation marks, this is the JWT you need to
access the weather API.

First attempt to access the weather API by expanding **/api/weather**, clicking
**Try it out**, and **Execute**. You should get response 401: Unauthorized.
Next, tell swagger to attach your authentication token to API requests. Scroll
to the top of the page and click on the button **Authorize**.

[Image 4]

An input prompt should appear asking you for the JWT you copied earlier. In the
text box enter without quotes: "**Bearer eyJhb...Py6gw**" where *eyJhb...Py6gw*
is *your copied token*. Then click **Authorize** and **Close**.

Now try accessing the weather API as before. This time you should receive a code
200: Success, with some random wather data in the body.

[Image 5]

## Interesting files



## Getting Started

To compile or continue development try this:

Install **git**, **dotnet SDK** and **Visual Studio Code**, then open a git console:

```
cd .\suitable\project\folder
git clone <address_to_this_repo>
cd ".\Minimal JWT Auth"
code .
```

To build and run enter in console:
```
cd .\AuthAPI
dotnet ef database update
dotnet build
dotnet watch run
```

## References

Inspiration, code snippets, etc.

* [Configure_Swagger_to_accept_Header_Authorization](https://www.freecodespot.com/blog/use-jwt-bearer-authorization-in-swagger/#VIII_Configure_Swagger_to_accept_Header_Authorization)