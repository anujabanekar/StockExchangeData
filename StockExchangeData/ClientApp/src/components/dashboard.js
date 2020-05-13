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
            name:""
        };
        this.populateStockData = this.populateStockData.bind(this);
        this.addItem = this.addItem.bind(this);
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
        this.setState({ modal: true, symbol: symbol, quantity : "", purchasePrice : "" });
    }

    modalClose() {
        this.setState({
            modalInputName: "",
            modal: false
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
                        <th>Price. </th>
                        <th>Quantity. </th>
                    </tr>
                </thead>
                <tbody>
                    {dashboardItems.map(m =>
                        <tr key={m.symbol}>
                            <td>{m.symbol}</td>
                            <td>{m.price}</td>
                            <td>{m.totalQuantity}</td>
                            <td>
                                <div className="form-group">
                                    <button onClick={e => this.modalOpen(m.symbol)} type="button">
                                        Open Modal
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
                        <h1 id="tabelLabel" >Stock Profile Summaries</h1>
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
                                    Add Item
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
                </div>
            </div>
        );
    }

    async populateStockData() {
        const response = await fetch('api/stockdata/GetProfileData');
        const data = await response.json();

        this.setState({ dashboardItems: data, loading: false });
/*
        axios.get(await `api/stockdata/GetProfileData`)
            .then(res => {
                this.setState({ dashboardItems: res, loading: false});
            });*/
    }

    async addItem(e) {
        // Prevent button click from submitting form
        e.preventDefault();

        // Create variables for our list, the item to add, and our form
        //  let list = this.state.dashboardItems;
        const newItem = document.getElementById("addInput");

        // If our input has a value
        if (newItem.value !== "") {

            var success = await fetch('api/stockdata/AddToDashBoard/' + newItem.value);
            if (success) {
                this.populateStockData();
            }

        }
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


        }
    }
}

