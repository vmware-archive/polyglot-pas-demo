package io.pivotal.fakeapply;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.stream.annotation.EnableBinding;
import org.springframework.cloud.stream.annotation.Output;
import org.springframework.cloud.stream.annotation.StreamListener;
import org.springframework.messaging.MessageChannel;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.support.GenericMessage;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.util.Arrays;
import java.util.List;
import java.util.Random;
import java.util.UUID;

@SpringBootApplication
@EnableBinding(ApplicationsBinding.class)
@EnableScheduling
public class FakeapplyApplication {

	private static final Logger LOG = LoggerFactory.getLogger(FakeapplyApplication.class);

	@Component("TimerSource")
	public static class TimerSource {

		private final MessageChannel applicationsOut;

		private List<String> names = Arrays.asList("Trump", "May", "Putin", "Merkel", "Holonde");
		private List<Long> amounts = Arrays.asList(1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 100000000L);

		public TimerSource(ApplicationsBinding binding) {
			this.applicationsOut = binding.sourceOfLoanApplications();
		}

		@Scheduled(fixedRate = 1000L)
		public void timerMessageSource() {
			String rName = names.get(new Random().nextInt(names.size()));
			Long rAmount = amounts.get(new Random().nextInt(amounts.size()));
			LoanApplication application = new LoanApplication(UUID.randomUUID().toString(), rName, rAmount);
			LOG.info("Created: {}", application);
			applicationsOut.send(new GenericMessage<LoanApplication>(application));
		}
	}


	@Component("ApplicationSink")
	public static class ApplicationSink {

		BigLoanSender sender;

		public ApplicationSink(BigLoanSender sender) {
			this.sender = sender;
		}

		@StreamListener(ApplicationsBinding.SOURCE_OF_LOAN_APPLICATIONS)
//		@SendTo(ApplicationsBinding.BIG_LOANS)
		public void process(LoanApplication application) {

			if (application.getAmount() > 10000L){
				LOG.info("ALERT! {} wants ${} in application {}", application.getName(), application.getAmount(), application.getUuid());
				sender.postBigLoan(application);
			} else {
				LOG.info("Dropped {}", application.getUuid());
			}
		}
	}

	@Component("BigLoanSender")
	public static class BigLoanSender{

		@SendTo(ApplicationsBinding.BIG_LOANS)
		public LoanApplication postBigLoan(LoanApplication application){
			LOG.info("Naughty! {} asked for {} in application {}", application.getName(), application.getAmount(), application.getUuid());
			return application;
		}
	}


	public static void main(String[] args) {
		SpringApplication.run(FakeapplyApplication.class, args);
	}

}

/**
 * This Application essentially reads it's own output, so Output happens first, the Input happens after.
 */

interface ApplicationsBinding{

  String SOURCE_OF_LOAN_APPLICATIONS = "source";
  String BIG_LOANS = "bigloans";


	/**
	 * Output happens first.
	 * The named binding "output" can also be seen in the configuration,
	 * for example spring.cloud.stream.bindings.*output*.destination=applications.
	 * These binding names have to match to work.
	 * @return
	 */
	@Output(SOURCE_OF_LOAN_APPLICATIONS)
	MessageChannel sourceOfLoanApplications();

	/**
	 * Output happens first.
	 * The named binding "output" can also be seen in the configuration,
	 * for example spring.cloud.stream.bindings.*output*.destination=applications.
	 * These binding names have to match to work.
	 * @return
	 */
	@Output(BIG_LOANS)
	MessageChannel sourceOfBigLoans();
}