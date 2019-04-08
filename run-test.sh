docker-compose up -d
java -jar time-source-kafka-2.1.0.RELEASE.jar --spring.cloud.stream.bindings.output.destination=ticktock
java -jar log-sink-kafka-2.1.1.RELEASE.jar --spring.cloud.stream.bindings.input.destination=ticktock --server.port=8081
