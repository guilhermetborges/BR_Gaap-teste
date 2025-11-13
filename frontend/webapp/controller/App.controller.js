sap.ui.define([
    "sap/ui/core/mvc/Controller",
    "sap/ui/model/json/JSONModel",
    "sap/m/MessageBox"
], function (Controller, JSONModel, MessageBox) {
    "use strict";

    return Controller.extend("brgfrontend.controller.App", {

        onInit: function () {
            this.page = 1;
            this.pageSize = 10;
            this.query = "";

            this.model = new JSONModel({
                todos: [],
                loading: false,
                pageText: ""
            });

            this.getView().setModel(this.model);
            this.loadData();
        },

    
        loadData: function () {

            const url =
                `/api/todos?page=${this.page}` +
                `&pageSize=${this.pageSize}` +
                `&title=${encodeURIComponent(this.query)}`;

            this.model.setProperty("/loading", true);

            fetch(url)
                .then(res => res.json())
                .then(data => {

                    // Corrige completed vindo como string
                    data.todos.forEach(t => {
                        t.completed = t.completed === true || t.completed === "true";
                    });

                    this.model.setData({
                        todos: data.todos,
                        total: data.total,
                        page: data.page,
                        pageText: `Página ${data.page}`,
                        loading: false
                    });

                })
                .catch(() => {
                    this.model.setProperty("/loading", false);
                    MessageBox.error("Erro ao carregar dados do servidor.");
                });
        },


        onSearchLive: function (oEvent) {
            clearTimeout(this._debounce);

            this._debounce = setTimeout(() => {
                this.query = oEvent.getParameter("newValue") || "";
                this.page = 1;
                this.loadData();
            }, 400);
        },

        onPrevPage: function () {
            if (this.page > 1) {
                this.page--;
                this.loadData();
            }
        },

        onNextPage: function () {
            this.page++;
            this.loadData();
        },

 
        onNavToDetail: function (oEvent) {
            var id = oEvent.getSource().getBindingContext().getObject().id;
            this.getOwnerComponent().getRouter().navTo("Detail", { id: id });
        },

  
        onToggleCompleted: function (oEvent) {
            var item = oEvent.getSource().getBindingContext().getObject();
            var selected = oEvent.getParameter("selected");

            this.model.setProperty("/loading", true);

            fetch(`/api/todos/${item.id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(selected)
            })
                .then(async res => {
                    if (!res.ok) {

                        const txt = await res.text();

                        MessageBox.error(
                            txt || "Não foi possível atualizar a tarefa.",
                            { title: "Ação não permitida" }
                        );

                        this.loadData();
                        return;
                    }

                    // Atualiza a lista após sucesso
                    this.loadData();
                })
                .catch(() => {
                    MessageBox.error("Erro de conexão com o servidor.");
                    this.model.setProperty("/loading", false);
                });
        }
    });
});
