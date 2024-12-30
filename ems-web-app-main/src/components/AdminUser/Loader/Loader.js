import React, { Component } from "react";
import "../Loader/loader.css";
class Loader extends Component {
  constructor(props) {
    super(props);
  }
  render() {
    return (
      <div id="preloader">
        <div id="loader"></div>
      </div>
    );
  }
}
export default Loader;
