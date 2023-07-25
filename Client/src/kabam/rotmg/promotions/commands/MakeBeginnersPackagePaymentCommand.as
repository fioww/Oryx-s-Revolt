﻿package kabam.rotmg.promotions.commands {
import kabam.lib.tasks.TaskMonitor;
import kabam.rotmg.account.core.PaymentData;
import kabam.rotmg.account.core.services.MakePaymentTask;
import kabam.rotmg.promotions.model.BeginnersPackageModel;

public class MakeBeginnersPackagePaymentCommand {

    [Inject]
    public var data:PaymentData;
    [Inject]
    public var task:MakePaymentTask;
    [Inject]
    public var monitor:TaskMonitor;
    [Inject]
    public var model:BeginnersPackageModel;


    // rofl
    public function execute():void {
        this.monitor.add(this.task);
        this.task.start();
        this.model.markAsPurchased();
    }


}
}
