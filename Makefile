generate:
	dotnet ef migrations add $(name) --project DepoQuick.DataAccess --startup-project DepoQuick.Frontend
migrate:
	dotnet ef database update --project DepoQuick.DataAccess --startup-project DepoQuick.Frontend
