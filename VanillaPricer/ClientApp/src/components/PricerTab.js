import React, { Component } from 'react';

export class PricerTab extends Component {
    static displayName = PricerTab.name;

    constructor(props) {
        super(props);
        this.state = { price: 0, type: "call", contracts: [] };
        this.priceContract = this.priceContract.bind(this);
    }

    async priceContract() {
        const response = await fetch('pricer/price');
        const data = await response.json();
        console.info(data);
        this.setState({ price: data });
    }

    async getContracts() {
        const response = await fetch('pricer/contracts');
        const data = await response.json();
        console.info(data);
        this.setState({ contracts: data });
    }

    async switchToCall() {
        this.setState({ type: "call" });
    }

    async switchToPut() {
        this.setState({ type: "put" });
    }

    render() {
        return (
            <div>
                <div>
                    <h1>Contracts</h1>
                    <p aria-live="polite">Contracts: </p>
                    {this.state.contracts.map(contractName => <p>{contractName}</p>)}
                    <button className="btn btn-primary base-margin" onClick={async () => { this.getContracts() }}>Get Contracts</button>
                </div>
                <div>
                    <h1>Pricer</h1>

                    <p>Princing single contract:.</p>

                    <p aria-live="polite">Price: <strong>{this.state.price}</strong></p>
                    <p aria-live="polite">Type: <strong>{this.state.type}</strong></p>

                    <button className="btn btn-primary base-margin" onClick={async () => { this.priceContract() }}>Price</button>
                    <button className="btn btn-primary base-margin" onClick={async () => { this.switchToCall() }}>Call</button>
                    <button className="btn btn-primary base-margin" onClick={async () => { this.switchToPut() }}>Put</button>
                </div>
            </div>
        );
    }
}
