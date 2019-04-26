package com.robobank.loanchecker;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Service
public class LoanCheckService {

  private static final Logger LOGGER = LoggerFactory.getLogger(LoanCheckService.class);

  private final Long maxLoanAmount;
  private final List<String> blacklist;
  private final Map<String, LoanApplication> approvedLoans;
  private final Map<String, LoanApplication> rejectedLoans;

  @Autowired
  public LoanCheckService(@Value("${max-loan-amount:100}") Long loanThreshold, LoansConfiguration loansConfiguration) {
    this.maxLoanAmount = loanThreshold;
    this.blacklist = loansConfiguration.getNaughtyList();

    LOGGER.info("The maximum loan is: {}", maxLoanAmount);
    LOGGER.info("The Naughty List has {} entries...", this.blacklist.size());
    this.blacklist.forEach(person -> LOGGER.info("'{}' is on the Naughty List", person));

    this.approvedLoans = new HashMap<>();
    this.rejectedLoans = new HashMap<>();
  }


  public LoanApplication validateLoan(LoanApplication application) {
    if (null == application.getStatus() || !application.getStatus().equals(Statuses.PENDING)) {
      RuntimeException e = new IllegalStateException("The application must have a Status of PENDING, yours was: " + application.getStatus());
      LOGGER.error(e.getMessage(), e);
      throw e;
    }

    if (application.getAmount() > maxLoanAmount) {
      reject(application);
      return application;
    }

    if (blacklist.contains(application.getName().toLowerCase())) {
      reject(application);
      return application;
    }

    approve(application);
    return application;
  }

  private void reject(LoanApplication application) {
    application.setStatus(Statuses.REJECTED.name());
    LOGGER.info("Rejected application id: {} from: {}", application.getId(), application.getName());
    this.rejectedLoans.put(application.getId(), application);
  }

  private void approve(LoanApplication application) {
    application.setStatus(Statuses.APPROVED.name());
    LOGGER.info("Approved application id: {} from: {}", application.getId(), application.getName());
    this.approvedLoans.put(application.getId(), application);
  }

  public List<LoanApplication> getAll(){
    LOGGER.info("Listing all Loan Applications...");
    List<LoanApplication> applications = new ArrayList<>();

    applications.addAll(approvedLoans.values());
    applications.addAll(rejectedLoans.values());

    applications.forEach(loan -> LOGGER.info("Application: {} status: {}", loan.getId(), loan.getStatus()));

    return applications;
  }
}
