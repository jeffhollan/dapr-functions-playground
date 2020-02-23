#/bin/bash

# install dapr
dapr init --kubernetes

# configure state store
helm install redis stable/redis
password=$(kubectl get secret --namespace default redis -o jsonpath="{.data.redis-password}" | base64 --decode)
# wait for ready
# update password in redis.yaml with $password
kubectl apply -f redis.yaml

# enable zipkin tracing
kubectl apply -f tracing.yaml

# install rabbitMq
helm install rabbitmq --set rabbitmq.username=user,rabbitmq.password=PASSWORD stable/rabbitmq
# wait for ready
kubectl apply -f rabbitmq.yaml

