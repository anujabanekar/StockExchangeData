import React, { Component } from 'react';

export class dashboard extends Component {
    static displayName = dashboard.name;

    constructor(props) {
        super(props);
        this.state = { dashboardItems: [], loading: true };
    }

    componentDidMount() {
        this.populateStockData();
    }

    static renderForecastsTable(dashboardItems) {

        if (dashboardItems == null)
            return;
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Symbol. </th>
                    </tr>
                </thead>
                <tbody>
                    {dashboardItems.map(m =>
                        <tr key={m.symbol}>
                            <td>{m.symbol}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : dashboard.renderForecastsTable(this.state.dashboardItems);

        return (          


            <div className="content">
               <div className="container">
                    <div>
                        <h1 id="tabelLabel" >Stock Summaries</h1>
                        <p>This component demonstrates fetching data from the server.</p>
                        {contents}
                    </div>
                    <hr />
                    <section className="section">
                        <form className="form" id="addItemForm">
                            <input
                                type="text"
                                className="input"
                                id="addInput"
                                id="addInput"
                                placeholder="Something that needs ot be done..."
                            />
                            <button className="button is-info" onClick={this.addItem}>
                                Add Item
        </button>
                        </form>
                    </section>
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
        if (newItem.value != "") {

            var success = await fetch('api/stockdata/AddToDashBoard/' + newItem.value);
            if (success) {
                window.location.reload(true);
            }

        } 
    }
}
