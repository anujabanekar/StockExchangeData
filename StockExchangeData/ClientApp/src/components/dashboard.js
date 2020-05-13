import React, { Component } from 'react';
import Modal from './common/Modal';

export class Dashboard extends Component {
    static displayName = Dashboard.name;

    constructor(props) {
        super(props);
        this.state = {
            dashboardItems: [], loading: true, show: false, modal: false,
            purchasePrice: "",
            quantity: "",
            symbol: "",
            name: "",
            portfolio: "",
            calculatePortfolioValueFlag: true,
            editStockFlag: false,
            stockData: null
        };
        this.populateStockData = this.populateStockData.bind(this);
        this.addItem = this.addItem.bind(this);
        this.getStockInformation = this.getStockInformation.bind(this);
        this.deleteStockValue = this.deleteStockValue.bind(this);
    }

    handleChange(e) {
        const target = e.target;
        const name = target.name;

        const value = target.value;

        this.setState({
            [name]: value
        });
    }

    handleSubmit(e) {
        this.modalClose();
        this.editItem(e);
    }

    modalOpen(symbol) {
        this.setState({ modal: true, symbol: symbol, quantity: "", purchasePrice: "" });
    }

    stockmodalOpen(symbol) {
        this.setState({ editStockFlag: true, symbol: symbol });
        this.getStockInformation(symbol);
    }

    modalClose() {
        this.setState({
            modalInputName: "",
            modal: false,
            editStockFlag: false
        });
    }

    componentDidMount() {
        this.populateStockData();
    }

    renderForecastsTable(dashboardItems) {

        if (dashboardItems == null)
            return;
        return (

            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Symbol. </th>
                        <th>Current Price. </th>
                        <th>Total Quantity. </th>
                        <th>Total Price. </th>
                    </tr>
                </thead>
                <tbody>
                    {dashboardItems.map(m =>
                        <tr key={m.symbol}>
                            <td><a href onClick={e => this.stockmodalOpen(m.symbol)} type="button">
                                {m.symbol}
                            </a></td>
                            <td>{m.price}</td>
                            <td>{m.totalQuantity}</td>
                            <td>${m.totalPrice}</td>
                            <td>
                                <div className="form-group">
                                    <button onClick={e => this.modalOpen(m.symbol)} type="button">
                                        +
                                    </button>
                                </div>

                            </td>
                        </tr>
                    )}
                </tbody>
            </table>

        );
    }



    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderForecastsTable(this.state.dashboardItems);

        return (
            <div className="content">
                <div className="container">
                    <div>
                        <div>
                            <h1 id="tabelLabel" >Portfolio Value : ${this.state.portfolio}</h1>
                        </div>
                        <p>This component demonstrates fetching data from the server.</p>


                        <section className="section">
                            <form className="form" id="addItemForm">
                                <input
                                    type="text"
                                    className="input"
                                    id="addInput"
                                    placeholder="Add stock name."
                                />
                                <button className="button is-info" onClick={this.addItem}>
                                    +
                                </button>
                            </form>
                        </section>
                        {contents}

                    </div>
                    <div>
                        <Modal show={this.state.modal} handleClose={e => this.modalClose(e)} children={this.state.symbol} >
                            <h2>Hello Modal</h2>
                            <form className="form" id="editItemForm">
                                <div className="form-group">
                                    <label>Stock name: </label>
                                    <input
                                        type="text"
                                        value={this.state.symbol}
                                        disabled
                                    />

                                </div>
                                <div className="form-group">
                                    <label>Enter Quantity:</label>
                                    <input
                                        name="quantity"
                                        type="text"
                                        value={this.state.quantity}
                                        className="input"
                                        onChange={e => this.handleChange(e)}
                                    />
                                </div>
                                <div className="form-group">
                                    <label>Enter Purchase Price:</label>
                                    <input
                                        name="purchasePrice"
                                        type="text"
                                        value={this.state.purchasePrice}
                                        className="input"
                                        onChange={e => this.handleChange(e)}
                                    />
                                </div>

                                <div className="button is-info">
                                    <button onClick={e => this.handleSubmit(e)} type="button">
                                        Save
                                </button>
                                </div>
                            </form>
                        </Modal>
                    </div>

                    <div>
                        {this.state.stockData != null && this.state.stockData.map(m =>
                            <Modal show={this.state.editStockFlag} handleClose={e => this.modalClose(e)}  >
                                <h2>{m.symbol} Stock Info</h2>
                                <form className="form" id="editItemForm">
                                    <table className='table table-striped' aria-labelledby="tabelLabel">
                                        <thead>
                                            <tr>
                                                <th>Quantity. </th>
                                                <th>Price. </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {m.addPurchase.map(purchase =>
                                                <tr key={purchase.quantity}>
                                                    <td>{purchase.quantity}</td>
                                                    <td>${purchase.purchasePrice}</td>   
                                                    <td>
                                                        {console.log(purchase.key)}
                                                        <div className="form-group">
                                                            <button onClick={e => this.deleteStockValue(m.symbol, purchase.key)} type="button">
                                                                -
                                                            </button>
                                                        </div>

                                                    </td>
                                                </tr>
                                            )}
                                        </tbody>
                                    </table>                                 
                                  

                                </form>
                            </Modal>
                        )}
                    </div>
                </div>
            </div>
        );
    }



    async populateStockData() {
        const response = await fetch('api/stockdata/GetProfileData');
        const data = await response.json();

        this.setState({ dashboardItems: data, loading: false, calculatePortfolioValueFlag: false });

        this.calculatePortfolioValue();
    }

    async getStockInformation(symbol) {
        const response = await fetch('api/stockdata/GetPortfolioValue/' + symbol);
        const data = await response.json();
        console.log(data);
        this.setState({ stockData: data });
    }

    async deleteStockValue(symbol, id) {

        console.log(id);
        const response = await fetch('api/stockdata/DeleteStockPurchase/' + symbol + '/' + id);
        const data = await response.json();

        if (data) {
            this.getStockInformation(symbol);
        }

    }

    calculatePortfolioValue() {

        const items = this.state.dashboardItems;

        const totalPrice = items.reduce((totalPrice, item) => totalPrice + item.totalPrice, 0);

        console.log(totalPrice);
        this.setState({ portfolio: totalPrice });
    }

    async addItem(e) {
        // Prevent button click from submitting form
        e.preventDefault();

        // Create variables for our list, the item to add, and our form
        //  let list = this.state.dashboardItems;
        const stockName = document.getElementById("addInput");
        // If our input has a value
        if (stockName.value !== "") {

            var success = await fetch('api/stockdata/AddToDashBoard/' + stockName.value);
            if (success) {
                this.populateStockData();
            }

        }

        //set txt to empty
        document.getElementById("addInput").value = "";
    }

    async editItem(e) {
        // Prevent button click from submitting form
        e.preventDefault();
        // Create variables for our list, the item to add, and our form
        //  let list = this.state.dashboardItems;
        //  var url = 'api/stockdata/EditQuantity/' + this.state.symbol + ' / ' + this.state.quantity;
        // If our input has a value
        if (this.state.quantity !== "" && this.state.symbol !== "" && this.state.purchasePrice !== "") {

            var success = await fetch('api/stockdata/EditQuantity/' + this.state.symbol + '/' + this.state.quantity + '/' + this.state.purchasePrice);

            if (success) {
                this.populateStockData();
            }
        }
    }
}

