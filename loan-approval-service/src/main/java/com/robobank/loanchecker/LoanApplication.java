package com.robobank.loanchecker;


import java.util.Objects;

public class LoanApplication {

  private String id;
  private String name;
  private Statuses status;
  private long amount;

  public LoanApplication() {
  }

  public LoanApplication(String id, String name, long amount) {
    this.id = id;
    this.name = name;
    this.amount = amount;
    this.setStatus(Statuses.PENDING.name());
  }

  public String getId() {
    return id;
  }

  public String getName() {
    return name;
  }

  public Statuses getStatus() {
    return status;
  }

  public void setStatus(String status) {
    if (status.equals(Statuses.APPROVED.name())
            || status.equals(Statuses.DECLINED.name())
            || status.equals(Statuses.PENDING.name())
            || status.equals(Statuses.REJECTED.name())) {
      this.status = Statuses.valueOf(status);
    } else {
      throw new IllegalArgumentException("Cannot set the LoanApplication's status to " + status);
    }
  }

  public long getAmount() {
    return amount;
  }

  @Override
  public boolean equals(Object o) {
    if (this == o) return true;
    if (o == null || getClass() != o.getClass()) return false;
    LoanApplication that = (LoanApplication) o;
    return amount == that.amount &&
            id.equals(that.id) &&
            name.equals(that.name) &&
            Objects.equals(status, that.status);
  }

  @Override
  public int hashCode() {
    return Objects.hash(id, name, status, amount);
  }

  @Override
  public String toString() {
    return "LoanApplication{" +
            "id='" + id + '\'' +
            ", name='" + name + '\'' +
            ", status='" + status.name() + '\'' +
            ", amount=" + amount +
            '}';
  }
}
