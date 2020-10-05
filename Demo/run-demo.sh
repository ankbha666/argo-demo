# Install argo cli
curl -sLO https://github.com/argoproj/argo/releases/download/v2.11.1/argo-darwin-amd64.gz
gunzip argo-darwin-amd64.gz
chmod +x argo-darwin-amd64
mv ./argo-darwin-amd64 /usr/local/bin/argo
argo version

# Submit workflow
argo submit -n argo --watch argo-newsarticle-dag.yaml

# List workflow
argo List

# Resume workflow

argo resume <workflowId>

# Get Argo server external IP
kubectl get svc -n argo