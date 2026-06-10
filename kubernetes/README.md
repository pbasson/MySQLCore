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

minikube image build -t mysqlcore-api:latest -f src/MySQLCore.API/Dockerfile .
minikube image build -t mysqlcore-worker:latest -f worker/MySQLCore.Worker/Dockerfile .
```


Apply Persistent Volume:

```bash
kubectl apply -f env/kubernetes-env/configmap-env/mysqlcore-configmap.dev.yml
kubectl apply -f env/kubernetes-env/secret-env/mysqlcore-secret.dev.yml

kubectl apply -f kubernetes/persistentvolume-kube/database-kube/mysql-pv.yml

kubectl apply -f kubernetes/persistentvolume-kube/observability-kube/seq-pv.yml
kubectl apply -f kubernetes/persistentvolume-kube/observability-kube/tempo-pv.yml
kubectl apply -f kubernetes/persistentvolume-kube/observability-kube/grafana-pv.yml
kubectl apply -f kubernetes/persistentvolume-kube/observability-kube/prometheus-pv.yml

```

Apply Deployments:

```bash
# kubectl apply -f kubernetes/deployments-kube/database-kube/mysql-deployment.yml
kubectl apply -f kubernetes/deployments-kube/database-kube/
kubectl apply -f kubernetes/deployments-kube/middleware-kube/
kubectl apply -f kubernetes/deployments-kube/observability-kube/
kubectl apply -f kubernetes/deployments-kube/backend-kube/
```

Delete Deployments:

```bash
kubectl delete -f kubernetes/deployments-kube/database-kube/
kubectl delete -f kubernetes/deployments-kube/middleware-kube/
kubectl delete -f kubernetes/deployments-kube/observability-kube/
kubectl delete -f kubernetes/deployments-kube/backend-kube/
```




Apply local dev Ingress routes:

```bash
kubectl apply -f kubernetes/ingress-kube/
```

For Minikube, enable the ingress addon if needed:

```bash
minikube addons enable ingress
```

Add local host entries using the output from `minikube ip`:

```text
<minikube-ip> mysqlcore.local
<minikube-ip> grafana.mysqlcore.local
<minikube-ip> seq.mysqlcore.local
```

Then browse to:

```text
http://mysqlcore.local/swagger
http://grafana.mysqlcore.local
http://seq.mysqlcore.local
```


If you prefer port-forwarding instead of Ingress:

```bash
kubectl port-forward service/backend 5820:5820
kubectl port-forward service/grafana 3000:3000
kubectl port-forward service/seq 5341:80
```

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
