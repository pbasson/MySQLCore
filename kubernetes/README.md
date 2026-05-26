# MySQLCore Kubernetes dev setup

This is the local Kubernetes equivalent of the core Docker Compose runtime:

- MySQL
- RabbitMQ
- Redis
- Seq
- MySQLCore API
- MySQLCore Worker

Apply order:

Build the local app images first:

```bash
docker build -t mysqlcore-api:latest -f src/MySQLCore.API/Dockerfile . 
docker build -t mysqlcore-worker:latest -f worker/MySQLCore.Worker/Dockerfile .
```

Then apply the manifests:

```bash
kubectl apply -f env/kubernetes-env/mysqlcore-configmap.dev.yml
kubectl apply -f env/kubernetes-env/mysqlcore-secret.dev.yml

kubectl apply -f kubernetes/database-kube/mysql-pv.yml
kubectl apply -f kubernetes/database-kube/mysql-deployment.yml

kubectl apply -f kubernetes/middleware-kube/
kubectl apply -f kubernetes/observability-kube/seq-pv.yml
kubectl apply -f kubernetes/observability-kube/tempo-pv.yml
kubectl apply -f kubernetes/observability-kube/grafana-pv.yml
kubectl apply -f kubernetes/observability-kube/prometheus-pv.yml
kubectl apply -f kubernetes/observability-kube/
kubectl apply -f kubernetes/backend-kube/
```

For local clusters such as Docker Desktop or Minikube, expose the API with:

```bash
kubectl port-forward service/backend 5820:5820
```

Then browse to `http://localhost:5820/swagger`.

Notes:

- The API container listens on port `5820`, so the Kubernetes service targets `5820`.
- The API and worker use local images: `mysqlcore-api:latest` and `mysqlcore-worker:latest`.
- If you use Docker Desktop Kubernetes, the cluster can usually see images built by your local Docker daemon.
- If you use Minikube, build inside Minikube's Docker daemon first: `eval $(minikube docker-env)`, then run the two `docker build` commands.
- Secrets are in `stringData` for dev convenience. Use a real secret workflow for production.
- Seq stores events in `/mnt/data/seq` inside the local Kubernetes VM.
- Tempo stores traces in `/mnt/data/tempo` inside the local Kubernetes VM.
- Grafana stores dashboards and settings in `/mnt/data/grafana` inside the local Kubernetes VM.
- Prometheus stores metrics in `/mnt/data/prometheus` inside the local Kubernetes VM.
