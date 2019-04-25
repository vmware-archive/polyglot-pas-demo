docker run --publish 8761:8761 steeltoeoss/eureka-server

#docker run --public 7979:7979 steeltoeoss/hystrix-dashboard

#docker run --publish 6379:6379 steeltoeoss/redis

docker run --publish 9411:9411 steeltoeoss/zipkin


#RUN ON LINUX
sudo docker run -d -p 8888:8888 steeltoeoss/config-server \
	--spring.cloud.config.server.git.uri=https://github.com/benwilcock/polyglot-pas-demo.git \
	--spring.cloud.config.server.git.searchPaths=config/test \
	--spring.cloud.config.server.git.clone-on-start=true \
	--spring.cloud.config.server.git.skip-ssl-validation=true

#{name},   {profiles},    {label},   {version},   {state}
#env.Name, env.Profiles, env.Label, env.Version, env.State