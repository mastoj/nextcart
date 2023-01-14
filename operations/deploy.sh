#/bin/bash -e

kubectl get deploy nextcart
[ $? -eq 0 ] && kubectl delete deploy nextcart
kubectl get deploy nextcart-service
[ $? -eq 0 ] && kubectl delete deploy nextcart-service
kubectl apply -f deployment.yaml
