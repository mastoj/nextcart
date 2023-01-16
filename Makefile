.SILENT: ;
.DEFAULT_GOAL := help

help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

build-container: ## Build the container
	dotnet publish src/NextCart.Api --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
	dotnet publish src/NextCart.Service --os linux --arch x64 -c Release /t:PublishContainer

docker-run: ## Run the application in docker
	docker compose -f compose-app.yaml up

local-run: ## Run the application locally
	dotnet run --project src/NextCart.Api/NextCart.Api.csproj

minikube-start: ## Start minikube
	minikube start

minikube-publish-image: ## Publish images to minikube
	minikube image load nextcart-api:1.0.0 --overwrite
	minikube image load nextcart-service:1.0.0 --overwrite

kube-apply: ## Apply k8s manifests
	cd operations && ./deploy.sh

kube-tunnel: ## Run tunnel to minikube
	minikube service nextcart --url

k6-run: ## Run k6 load test
	NEXTCART_HOST=$$(minikube service nextcart --url) k6 run k6/index.js
