import * as React from "react";

interface IViewModeProps {
    placingOrder: boolean;
    switchView: (viewName: string) => void;
}

export class ViewMode extends React.Component<IViewModeProps, any> {

    public render() {
        const activeDiv = "anlab_form_style col-5 active-border active-text active-bg";
        const inactiveDiv = "anlab_form_style col-5";

        return (
            <div>
                <div className="row">
                    <div
                        className={this.props.placingOrder ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("order")}
                    >
                        <h3>Create New Order</h3>
                        <p>I'm ready to place an order with the lab</p>
                    </div>
                    <span className="dividing_span col-2 t-center align-middle">or</span>
                    <div
                        className={!this.props.placingOrder ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("browse")}
                    >
                        <h3>Browse</h3>
                        <p>I just want to browse the available tests and prices</p>
                    </div>
                </div>
            </div>
        );
    }

    private _handleChange = (viewName: string) => {
        this.props.switchView(viewName);
    }

}
