package io.pivotal.loancheck;


import java.util.Objects;

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

  @Override
  public boolean equals(Object o) {
    if (this == o) return true;
    if (o == null || getClass() != o.getClass()) return false;
    LoanApplication that = (LoanApplication) o;
    return amount == that.amount &&
            uuid.equals(that.uuid) &&
            Objects.equals(name, that.name);
  }

  @Override
  public int hashCode() {
    return Objects.hash(uuid, name, amount);
  }
}
