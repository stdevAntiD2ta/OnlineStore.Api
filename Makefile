.DEFAULT_GOAL 	:= help
.PHONY: clean, info

PORT        := 5001
ENDPOINT    := ""
PROJECTNAME := ContosoPets.Api
MODEL 		:= Model
DBCONTEXT	:= MainDb
MIGRATION 	:= "InitialCreate"

add-gitignore: ## Create gitignore
	dotnet new gitignore

get-endpoint-json: ## Request json output of an endpoint, use with ENDPOINT="<endpoint>"
	curl -k -s https://localhost:${PORT}/${ENDPOINT} | jq

build: ## Build project
	dotnet build

build-fast: ## Build project bypassing restoration of NuGet packages
	dotnet build --no-restore

run: ## Run Project in Production mode
	dotnet run
	
run-dev: ## Run Project in development mode
	dotnet run environment=development

run-bg: ## Run project in the background
	dotnet ./bin/Debug/net5.0/${PROJECTNAME}.dll > ${PROJECTNAME}.log &

kill-dotnet: ## Kill process wich GPID is dotnet
	kill $(pidof dotnet)

add-packages: ## dotnet add commands used
	dotnet add package MySql.Data.EntityFrameworkCore
	dotnet add package Microsoft.EntityFrameworkCore.Design
	dotnet add package Microsoft.EntityFrameworkCore.Tools
	dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
	dotnet add package Microsoft.EntityFrameworkCore.SqlServer
	dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
	dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
	dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson

scaffold-controller: ## Scaffold controller using MODEL="<model>" and DBCONTEXT="<DBContext>"
	dotnet aspnet-codegenerator controller -name ${MODEL}sController -async -api -m ${MODEL} -outDir Controllers

add-migration:
	dotnet ef migrations add ${MIGRATION}
	dotnet ef database update
	
help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'
