package io.pivotal.fakeapply;



public class LoanApplication {

  private String uuid, name;
  private long amount;

  public LoanApplication() {
  }

  public LoanApplication(String uuid, String name, long amount) {
    this.uuid = uuid;
    this.name = name;
    this.amount = amount;
  }

  public String getUuid() {
    return uuid;
  }

  public String getName() {
    return name;
  }

  public long getAmount() {
    return amount;
  }

  @Override
  public String toString() {
    return "LoanApplication{" +
            "uuid='" + uuid + '\'' +
            ", name='" + name + '\'' +
            ", amount=" + amount +
            '}';
  }
}
