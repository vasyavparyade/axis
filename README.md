# AxisUtility

## Publish to single file

```bash
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true
```

## Add new rule to camera

* Rule names:
  - fire
  - smoke

```bash
-r <rule_name> -c "<login>:<password>@<camera_ip>|<host_ip>"
```

## Add some rules

```dos
-r <rule_name> -f "<path_to_file>"
```

### File example

```bash
10.20.160.10;root;admin;10.20.165.85
10.20.160.11;root;admin;10.20.165.86
10.20.160.12;root;admin;10.20.165.87
```
