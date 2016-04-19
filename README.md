# GuidOne

.NET GUID Generation library behind the project http://guid.one

This library was built as a learning exercise to understand GUIDs - I've made the code available to anyone who is also interested under a permissive license.

This library implements the following GUID generation algorithms specified in [RFC 4122](http://www.ietf.org/rfc/rfc4122.txt) 

* Version 1: Date-time and MAC address GUID
* Version 3: MD5 hash & namespace
* Version 4: Random
* Version 5: HA-1 hash & namespace

Visit [my blog for a 3-part series](https://michael-mckenna.com/tag/guid/) that dives into the secret world of GUIDs: https://michael-mckenna.com/tag/guid/

##Usage

``` C#
var guidV1 = UUID.V1(); //Generate a V1 GUID for current time + random node
var guidV4 = UUID.V4(); //Generate a V4 GUID (random)
var guidV5 = UUID.V5(GuidNamespaces.DNS, "guid.one"); //Generate a V5 GUID for a particular domain

var guid = guidV4.AsGuid(); //Convert to a standard .NET Guid

```

_Note: This library was created just to investigate the inner workings of GUIDs and is not considered production ready_
