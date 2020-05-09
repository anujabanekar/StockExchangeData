import React, { Component } from 'react';
import EditPopup from './common/EditPopup';

export class Dashboard extends Component {
    static displayName = Dashboard.name;

    constructor(props) {
        super(props);
        this.state = { dashboardItems: [], loading: true, showPopup: false };
        this.state = {
            modal: false,
            name: "",
            modalInputName: ""
        };
    }

    componentDidMount() {
        this.populateStockData();
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
        this.setState({ name: this.state.modalInputName });
        this.modalClose();
    }

    modalOpen() {
        this.setState({ modal: true });
    }

    modalClose() {
        this.setState({
            modalInputName: "",
            modal: false
        });
    }

    togglePopup() {
        this.setState({
            showPopup: !this.state.showPopup
        });
    }

    static renderForecastsTable(dashboardItems) {

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
                            <td>{m.quantity}</td>
                            <td>
                                <button onClick={this.togglePopup}> Click To Launch Popup</button>
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
            : Dashboard.renderForecastsTable(this.state.dashboardItems);

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
                                    id="addInput"
                                    placeholder="Add stock name."
                                />
                                <button className="button is-info" onClick={this.addItem}>
                                    Add Item    
                                </button>
                                <div>

                                    {this.state.showPopup ?
                                        <EditPopup
                                            text='Click "Close Button" to hide popup'
                                            closePopup={this.togglePopup.bind(this)}
                                        />
                                        : null
                                    }
                                </div>  
                            </form>
                        </section>
                        {contents}
                    </div>
                  


                </div>
            </div>
        );
    }

    async populateStockData() {
        const response = await fetch('api/stockdata/GetProfileData');
        const data = await response.json();

        this.setState({ dashboardItems: data, loading: false });
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
                window.location.reload(true);
            }

        }
    }

    async editItem(e) {
        // Prevent button click from submitting form
        e.preventDefault();

        // Create variables for our list, the item to add, and our form
        //  let list = this.state.dashboardItems;
        const newItem = document.getElementById("addInput");

        // If our input has a value
        if (newItem.value !== "") {

            var success = await fetch('api/stockdata/AddToDashBoard/' + newItem.value);
            if (success) {
                window.location.reload(true);
            }

        }
    }
}

