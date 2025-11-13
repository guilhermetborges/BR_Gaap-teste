sap.ui.define([
    "sap/ui/core/mvc/Controller",
    "sap/ui/model/json/JSONModel"
], function (Controller, JSONModel) {
    "use strict";

    return Controller.extend("brgfrontend.controller.Detail", {

        onInit: function () {
            this.getOwnerComponent()
                .getRouter()
                .getRoute("Detail")
                .attachPatternMatched(this._onMatched, this);
        },

        _onMatched: function (oEvent) {
            const id = oEvent.getParameter("arguments").id;

            fetch(`/api/todos/${id}`)
                .then(res => res.json())
                .then(data => {
                    // garante que completed é boolean
                    data.completed = data.completed === true;

                    const model = new JSONModel({ todo: data });
                    this.getView().setModel(model, "todoModel");
                });
        },

        formatCompleted: function (value) {
            console.log("Formatter recebeu:", value);
            return value ? "Sim" : "Não";
        },

        onNavBack: function () {
            history.back();
        }

    });
});
