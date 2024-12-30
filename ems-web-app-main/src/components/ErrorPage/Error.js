import React, { Component } from "react";
import "./Error.css";
class Error extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <div className="errorBody">
        <section id="not-found">
          <div id="title"></div>
          <div class="circles">
            <p style={{ color: "black" }}>
              404
              <br />
              <small>
                PAGE NOT FOUND
                <br />
                <span style={{ wordBreak: "break-all" }}>
                  it looks like one of the developers fell asleep
                </span>
              </small>
            </p>
            <span class="circle big"></span>
            <span class="circle med"></span>
            <span class="circle small"></span>
          </div>
        </section>
      </div>
    );
  }
}
export default Error;
