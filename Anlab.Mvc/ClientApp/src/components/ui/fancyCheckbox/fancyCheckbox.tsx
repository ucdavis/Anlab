import * as React from "react";
import "./styles.scss";

export interface IProps {
  name?: string;
  checked: boolean;
  label?: string;
  labelClassName?: string;
  onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
  inputRef?: (element: HTMLInputElement) => void;
  labelOnLeft?: boolean;
}

export default class FancyCheckbox extends React.PureComponent<IProps, {}> {
  public static defaultProps: Partial<IProps> = {
    labelOnLeft: false,
  };

  public render() {
    const { labelOnLeft } = this.props;
    const label = <span className="label-material">{this.props.label}</span>;

    return (
      <div className="checkbox fancy">
        <label>
          <input
            type="checkbox"
            name={this.props.name}
            ref={this.props.inputRef}
            checked={this.props.checked}
            onChange={this.props.onChange}
          />
          {labelOnLeft && label}
          <span className="checkbox-material">
            <span className="check" />
          </span>
          {!labelOnLeft && label}
        </label>
      </div>
    );
  }
}
