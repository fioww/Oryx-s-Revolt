﻿package kabam.rotmg.account.web.commands {
import com.company.assembleegameclient.screens.CharacterSelectionAndNewsScreen;

import flash.display.Sprite;

import kabam.lib.tasks.BaseTask;
import kabam.rotmg.account.core.Account;
import kabam.rotmg.core.model.ScreenModel;
import kabam.rotmg.core.signals.InvalidateDataSignal;
import kabam.rotmg.core.signals.SetScreenWithValidDataSignal;
import kabam.rotmg.fame.view.FameView;
import kabam.rotmg.pets.data.PetsModel;

public class WebLogoutCommand {

    [Inject]
    public var account:Account;
    [Inject]
    public var invalidate:InvalidateDataSignal;
    [Inject]
    public var setScreenWithValidData:SetScreenWithValidDataSignal;
    [Inject]
    public var screenModel:ScreenModel;
    [Inject]
    public var petsModel:PetsModel;


    public function execute():void {
        this.account.clear();
        this.invalidate.dispatch();
        this.petsModel.clearPets();
        this.setScreenWithValidData.dispatch(this.makeScreen());
    }

    private function makeScreen():Sprite {
        if (this.screenModel.getCurrentScreenType() == FameView) {
            return (new CharacterSelectionAndNewsScreen());
        }
        return (new (((this.screenModel.getCurrentScreenType()) || (CharacterSelectionAndNewsScreen)))());
    }


}
}
