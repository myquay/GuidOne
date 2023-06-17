# GuidOne

.NET GUID Generation library behind the project http://guid.one

This library was built as a learning exercise to understand GUIDs - I've made the code available to anyone who is also interested under a permissive license.

This library implements the following GUID generation algorithms specified in [RFC 4122](http://www.ietf.org/rfc/rfc4122.txt) 

* Version 1: Date-time and MAC address GUID
* Version 3: MD5 hash & namespace
* Version 4: Random
* Version 5: SHA-1 hash & namespace

Visit my blog for a 3-part series that dives into the secret world of GUIDs: [https://michael-mckenna.com/what-the-guid](https://michael-mckenna.com/what-the-guid).

## Usage

### Generating GUIDs

``` C#
var guidV1 = Uuid.NewV1(); //Generate a V1 GUID for current time + random node
var guidV3 = Uuid.NewV3(GuidNamespaces.DNS, "guid.one"); //Generate a V3 GUID for a particular domain
var guidV4 = Uuid.NewV4(); //Generate a V4 GUID (random)
var guidV5 = Uuid.NewV5(GuidNamespaces.DNS, "guid.one"); //Generate a V5 GUID for a particular domain

var guid = guidV4.AsGuid(); //Convert to a standard .NET Guid
```

### Inspecting GUIDs

``` C#
var uuid = new Uuid(Guid.Parse("63b00000-bfde-11d3-b852-290676ece2d7")); // Parse a UUID

var version = uuid.Version;
var variant = uuid.Variant;
var version = uuid.Timestamp; //Only for timebased Guids
```

### Generating with no conflicts (locally)

The timestamp GUIDs can conflict if generated too quickly, the library supports a slower mode of generation that makes sure there's no duplicates (non-distributed!)


``` C#
var uuid = new Uuid(Guid.Parse("63b00000-bfde-11d3-b852-290676ece2d7")); // Parse a UUID

using (var v1Gen = new UuidV1Generator(generationMode: GenerationMode.NoDuplicates)){
  var unique = v1Gen.NewV1(now, PhysicalAddress.Parse("...")).AsGuid();
}
```

_Note: This library was created just to investigate the inner workings of GUIDs and is not considered production ready_
