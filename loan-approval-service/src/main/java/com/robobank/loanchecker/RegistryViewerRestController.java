package com.robobank.loanchecker;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cloud.client.ServiceInstance;
import org.springframework.cloud.client.discovery.DiscoveryClient;
import org.springframework.context.annotation.Profile;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@Profile("cloud")
public class RegistryViewerRestController {
  @Autowired
  private DiscoveryClient discoveryClient;

  @RequestMapping("/service-instances/{applicationName}")
  public List<ServiceInstance> serviceInstancesByApplicationName(
          @PathVariable String applicationName) {
    return this.discoveryClient.getInstances(applicationName);
  }
}
