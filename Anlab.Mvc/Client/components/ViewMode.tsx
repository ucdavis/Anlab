import * as React from "react";

interface IViewModeProps {
    placingOrder: boolean;
    switchView: (b: boolean) => void;
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
                        onClick={() => this._handleChange()}
                    >
                        <h3>Create Order</h3>
                    </div>
                    <span className="dividing_span col-2 t-center align-middle">or</span>
                    <div
                        className={!this.props.placingOrder ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange()}
                    >
                        <h3>Browse</h3>
                    </div>
                </div>
            </div>
        );
    }

    private _handleChange = () => {
        this.props.switchView(!this.props.placingOrder);
    }

}
