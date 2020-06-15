REM Create cluster
kind create cluster

REM Deploy Argo
kubectl create namespace argo
kubectl apply -n argo -f https://raw.githubusercontent.com/argoproj/argo/stable/manifests/install.yaml
kubectl create rolebinding default-admin --clusterrole=admin --serviceaccount=default:default
kubectl delete configmap -n argo workflow-controller-configmap
kubectl create configmap -n argo workflow-controller-configmap --from-literal=config="containerRuntimeExecutor: k8sapi"
pause
kubectl -n argo port-forward deployment/argo-server 8085:2746