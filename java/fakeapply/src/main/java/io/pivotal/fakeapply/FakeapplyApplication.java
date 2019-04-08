package io.pivotal.fakeapply;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.stream.annotation.EnableBinding;
import org.springframework.cloud.stream.annotation.Output;
import org.springframework.kafka.support.KafkaHeaders;
import org.springframework.messaging.Message;
import org.springframework.messaging.MessageChannel;
import org.springframework.messaging.support.MessageBuilder;
import org.springframework.stereotype.Component;

import java.util.Arrays;
import java.util.List;
import java.util.Random;
import java.util.UUID;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

@SpringBootApplication
@EnableBinding(ApplicationsBinding.class)
public class FakeapplyApplication {

	private static final Logger LOG = LoggerFactory.getLogger(FakeapplyApplication.class);

	@Component
	public static class ApplicationEventsSource implements ApplicationRunner{

		private final MessageChannel applicationsOut;

		public ApplicationEventsSource(ApplicationsBinding binding) {
			this.applicationsOut = binding.applicationsOut();
		}

		@Override
		public void run(ApplicationArguments args) throws Exception {

			List<String> names = Arrays.asList("Trump", "May", "Putin", "Merkel", "Holonde");
			List<Long> amounts = Arrays.asList(1000L, 10000L, 100000L, 1000000L, 10000000L);

			Runnable runnable = () -> {

				String rName = names.get(new Random().nextInt(names.size()));
				Long rAmount = amounts.get(new Random().nextInt(amounts.size()));

				LoanApplication application = new LoanApplication(UUID.randomUUID().toString(), rName, rAmount);

				Message applicationEvent = MessageBuilder
								.withPayload(application)
								.setHeader(KafkaHeaders.MESSAGE_KEY, application.getUuid().getBytes())
								.build();

				try {
					this.applicationsOut.send(applicationEvent);
					LOG.info("Sending Loan Application: {}", application);
				} catch (Exception e) {
					LOG.error("There was a problem.", e);
				}
			};
			Executors.newScheduledThreadPool(1).scheduleAtFixedRate(runnable, 1, 1, TimeUnit.SECONDS);
		}
	}

	public static void main(String[] args) {
		SpringApplication.run(FakeapplyApplication.class, args);
	}

}

interface ApplicationsBinding{

	@Output("output")
	MessageChannel applicationsOut();
}