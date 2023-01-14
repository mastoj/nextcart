.SILENT: ;
.DEFAULT_GOAL := help

help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

build-container: ## Build the container
	dotnet publish src/NextCart.Api --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer
	dotnet publish src/NextCart.Service --os linux --arch x64 -c Release /t:PublishContainer

docker-run-api: ## Run the container
	docker run -it -p 5000:80 -p 8090:8090 --rm --env-file src/NextCart.Api/.env.docker --add-host host.docker.internal:host-gateway --name nextcart-api nextcart-api:1.0.0

docker-run-service: ## Run the container
	docker run -it -p 8091:8091 --rm --env-file src/NextCart.Service/.env.docker --add-host host.docker.internal:host-gateway --name nextcart-service nextcart-service:1.0.0

local-run: ## Run the application locally
	dotnet run --project src/NextCart.Api/NextCart.Api.csproj

minikube-start: ## Start minikube
	minikube start


minikube-publish-image: minikube-start ## Publish images to minikube
	minikube image load nextcart-api:1.0.0
	minikube image load nextcart-service:1.0.0

kube-apply: minikube-publish-image ## Apply k8s manifests
	kubectl apply -f operations/deployment.yaml
