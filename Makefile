.SILENT: ;
.DEFAULT_GOAL := help

help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

build-container: ## Build the container
	dotnet publish src/NextCart.Api --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer

docker-run: ## Run the container
	docker run -it -p 5000:80 --rm --env-file src/NextCart.Api/.env.local --name nextcart-api nextcart-api:1.0.0