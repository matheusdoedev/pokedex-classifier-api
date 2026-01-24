REGISTRY=

dev:
	dotnet watch run

start:
	dotnet run

deploy:
	docker build -t pokedex-classifier:latest .
	docker tag pokedex-classifier:latest $(REGISTRY)/pokedex-classifier:latest
	docker push $(REGISTRY)/pokedex-classifier:latest