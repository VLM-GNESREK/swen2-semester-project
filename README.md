# TourPlanner

https://github.com/VLM-GNESREK/swen2-semester-project

## Version

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.2.

## Angular server

To start the front end, run:

```bash
ng serve
```
You might need to install missing libraries, so use in:
TourPlanner.FrontEnd
```
npm install
```
## .Net Server/API
You need to install\
RUN IN ROOT OF DIR
```
dotnet tool install --global dotnet-ef  
```  
RUN IN ROOT OF DIR
```
dotnet ef migrations add InitialCreate --project TourPlanner.DAL --startup-project TourPlanner.API
```
```
dotnet ef database update --project TourPlanner.DAL --startup-project TourPlanner.API
```


Add secrets
RUN IN Tourplanner.API
```
JwtSettings:SecretKey = 8UhCaMaoB17EkOZDWvWMFMABVdC1Y7iBknfr8f2lwvftRCS84ZtoXkbLeJelDsvYudJogpUiQmd5Gv8u7runkMas7xOHGrkMYTQwzL03oRSstcmcmXUSnTxGm2hHI95j\
```
```
JwtSettings:Issuer = TourPlannerAPI\
```
```
JwtSettings:Audience = TourPlannerFrontend\
```
```
ConnectionStrings:PostgresConnection = Host=localhost;Port=5432;Database=tourplanner_db;Username=postgres;Password=F4chh0chschul3T3chn1kumW13n2026#!?\
```
RUN IN Tourplanner.BL
```
OPENROUTE_APIKEY=eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6ImNlNmNiZWUxYTZjOTRhNWJhN2YyNGY4ODJjOWI2MzBhIiwiaCI6Im11cm11cjY0In0=\
```

Then you can simply run Program.cs
## Questions
If you have further questions, don't, thanks
