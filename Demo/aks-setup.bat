REM Create AKS cluster
az group create --name news-article-store-rg --location australiaeast
az aks create -n argo-demo --node-count 1 --node-vm-size Standard_B2s --load-balancer-sku basic --node-osdisk-size 32 --resource-group news-article-store-rg --generate-ssh-keys
az aks get-credentials --resource-group news-article-store-rg --name argo-demo

REM Install Argo
kubectl create namespace argo
kubectl apply -n argo -f https://raw.githubusercontent.com/argoproj/argo/stable/manifests/install.yaml
kubectl create rolebinding default-admin --clusterrole=admin --serviceaccount=default:default
kubectl delete configmap -n argo workflow-controller-configmap
kubectl create configmap -n argo workflow-controller-configmap --from-literal=config="containerRuntimeExecutor: k8sapi"

REM Change Argo server service type to Loadbalancer
kubectl patch service argo-server -n argo -p "{\"spec\": { \"type\": \"LoadBalancer\" } }"