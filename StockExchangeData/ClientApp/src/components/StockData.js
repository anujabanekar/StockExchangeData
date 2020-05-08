import React, { Component } from 'react';

export class StockData extends Component {
    static displayName = StockData.name;

  constructor(props) {
    super(props);
      this.state = { marketsummary: [], loading: true };
  }

  componentDidMount() {
    this.populateStockData();
  }

    static renderForecastsTable(marketsummary) {

        if (marketsummary == null)
            return;
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
                    <th>FullExchangeName</th>
            <th>Symbol. </th>
          </tr>
        </thead>
        <tbody>
                {marketsummary.map(m =>
                    <tr key={m.fullExchangeName}>
                        <td>{m.fullExchangeName}</td>
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
        : StockData.renderForecastsTable(this.state.marketsummary);

    return (
      <div>
        <h1 id="tabelLabel" >Stock Summaries</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateStockData() {
    const response = await fetch('stockdata');
      const data = await response.json();
      
    this.setState({ marketsummary: data, loading: false });
  }
}
