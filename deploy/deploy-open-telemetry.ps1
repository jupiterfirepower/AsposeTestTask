# this deploys open telemetry collector exporter to applciation insights
kubectl apply -f .\deploy\open-telemetry-collector.yaml
# this configures open telemetry for tracing
kubectl apply -f .\deploy\configuration-open-telemetry.yaml