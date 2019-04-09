package io.pivotal.fakeapply;

import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.streams.KeyValue;
import org.apache.kafka.streams.kstream.*;
import org.apache.kafka.streams.kstream.internals.KTableAggregate;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.stream.annotation.EnableBinding;
import org.springframework.cloud.stream.annotation.Input;
import org.springframework.cloud.stream.annotation.Output;
import org.springframework.cloud.stream.annotation.StreamListener;
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

	/**
	 * This component is PRODUCING LoanApplication messages and sending them onto the ApplicationsOut
	 * message channel. The message channel is defined in the interface ApplicationsBinding.
	 */
	@Component
	public static class ApplicationEventsSource implements ApplicationRunner{

		private final MessageChannel applicationsOut;

		public ApplicationEventsSource(ApplicationsBinding binding) {
			this.applicationsOut = binding.applicationsOut();
		}

		@Override
		public void run(ApplicationArguments args) throws Exception {

			List<String> names = Arrays.asList("Trump", "May", "Putin", "Merkel", "Holonde");
			List<Long> amounts = Arrays.asList(1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 100000000L);

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
					LOG.info("Sending LoanApplication: {}", application);
				} catch (Exception e) {
					LOG.error("There was a problem.", e);
				}
			};
			Executors.newScheduledThreadPool(1).scheduleAtFixedRate(runnable, 1, 1, TimeUnit.SECONDS);
		}
	}

	/**
	 * This component is CONSUMING LoanApplication messages from the ApplicationsIn
	 * message channel. The message channel is defined in the interface ApplicationsBinding.
	 */
	@Component
	public static class ApplicationsConsumer{

		@StreamListener
		public void process(
						@Input(ApplicationsBinding.INPUT) KStream<Object, LoanApplication> events){

		  LOG.info("Received: {}", events.peek((key, value) -> LOG.info("Received LoanApplication: {}", value)));

			KStream<Object, LoanApplication> bigLoans = events
							.filter((key, value) -> value.getAmount() > 10000);

			bigLoans
							.foreach((key, value) -> LOG.info("Alert! {}'s loan request was over the $10K limit at ${}", value.getName(), value.getAmount()));

			KTable<?, ?> bigLoanTable = bigLoans
							.map((key, value) -> new KeyValue<>(value.getName().getBytes(), String.valueOf(value.getAmount()).getBytes()))
							.groupByKey()
							.count(Materialized.as(ApplicationsBinding.VIEW));
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

  String OUTPUT = "output";
  String INPUT = "input";
  String VIEW = "view";


	/**
	 * Output happens first.
	 * The named binding "output" can also be seen in the configuration,
	 * for example spring.cloud.stream.bindings.*output*.destination=applications.
	 * These binding names have to match to work.
	 * @return
	 */
	@Output(OUTPUT)
	MessageChannel applicationsOut();

	/**
	 * Input happens second.
	 * The named binding "input" can also be seen in the configuration,
	 * for example spring.cloud.stream.bindings.*input*.destination=applications.
	 * These binding names have to match to work.
	 */
	@Input(INPUT)
	KStream<String, LoanApplication> applicationsIn();
}