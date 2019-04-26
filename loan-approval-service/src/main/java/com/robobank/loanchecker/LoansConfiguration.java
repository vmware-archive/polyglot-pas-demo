package com.robobank.loanchecker;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.context.annotation.Configuration;
import org.springframework.validation.annotation.Validated;
import java.util.List;

@Configuration
@ConfigurationProperties(prefix = "approvals")
@Validated
public class LoansConfiguration {

  private List<String> naughtylist;

  public void setNaughtyList(List<String> naughtylist) {
    this.naughtylist = naughtylist;
  }

  public List<String> getNaughtyList() {
    return this.naughtylist;
  }

}